using System.Transactions;
using Ginecologia.Modules.Pacientes.Application;

namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class ConvertProspectUseCase(IProspectRepository prospects, CreatePatientUseCase createPatient)
{
    public async Task<ProspectDto> ExecuteAsync(Guid id, ConvertProspectCommand command, CancellationToken cancellationToken = default)
    {
        var prospect = await prospects.FindByIdAsync(id, cancellationToken) ?? throw new ProspectNotFoundException(id);
        if (prospect.Estado == Domain.ProspectStatus.Convertido)
            throw new InvalidOperationException("El prospecto ya fue convertido en paciente.");

        // TransactionScope: las dos escrituras (paciente nuevo + prospecto marcado como
        // convertido) viven en DbContext/esquemas distintos. Igual que en Usuarios+Auth (Fase 5),
        // esto no promueve a una transaccion distribuida porque las conexiones no quedan abiertas
        // simultaneamente; verificado empiricamente en esta misma base. Si falla cualquiera de las
        // dos escrituras, ninguna queda confirmada: nunca un paciente huerfano sin su prospecto
        // marcado, ni viceversa.
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var patient = await createPatient.ExecuteAsync(new CreatePatientCommand(
            prospect.NombreCompleto,
            command.FechaNacimiento,
            prospect.Telefono,
            prospect.Email,
            command.DocumentoIdentidad,
            command.Alergias,
            command.AntecedentesPatologicosPersonales,
            null,
            null), cancellationToken);

        prospect.MarkAsConverted(patient.Id);
        await prospects.SaveChangesAsync(cancellationToken);
        transaction.Complete();
        return ProspectDto.From(prospect);
    }
}
