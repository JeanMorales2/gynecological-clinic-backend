using Ginecologia.Modules.Auth.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.Auth;

public class AuthAccountTests
{
    [Fact]
    public void Create_ConEmailValido_NormalizaAMinusculasYRecortaEspacios()
    {
        var account = AuthAccount.Create("  Doctora.PRUEBA@Clinica.com  ", "hash", AuthRoles.Ginecologa);

        Assert.Equal("doctora.prueba@clinica.com", account.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sin-arroba")]
    [InlineData("   ")]
    public void Create_ConEmailInvalido_Lanza(string email)
    {
        Assert.Throws<ArgumentException>(() => AuthAccount.Create(email, "hash", AuthRoles.Admin));
    }

    [Fact]
    public void Create_ConRolInvalido_Lanza()
    {
        Assert.Throws<ArgumentException>(() => AuthAccount.Create("a@b.com", "hash", "SuperAdmin"));
    }

    [Fact]
    public void SetPasswordHash_ConHashVacio_Lanza()
    {
        var account = AuthAccount.Create("a@b.com", "hash", AuthRoles.Admin);

        Assert.Throws<ArgumentException>(() => account.SetPasswordHash(" "));
    }

    [Fact]
    public void SetPasswordHash_ConHashValido_LoReemplaza()
    {
        var account = AuthAccount.Create("a@b.com", "hash-original", AuthRoles.Admin);

        account.SetPasswordHash("hash-nuevo");

        Assert.Equal("hash-nuevo", account.PasswordHash);
    }

    [Fact]
    public void Deactivate_y_Activate_CambianElEstado()
    {
        var account = AuthAccount.Create("a@b.com", "hash", AuthRoles.Admin);

        account.Deactivate();
        Assert.False(account.IsActive);

        account.Activate();
        Assert.True(account.IsActive);
    }
}
