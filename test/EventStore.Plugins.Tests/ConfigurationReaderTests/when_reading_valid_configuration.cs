using Xunit;

namespace EventStore.Plugins.Tests.ConfigurationReaderTests;

public class when_reading_valid_configuration {
    [Fact]
    public void should_return_correct_options() {
        var settings = ConfigParser.ReadConfiguration<LdapsSettings>
            (Path.Combine("ConfigurationReaderTests", "valid_node_config.yaml"), "LdapsAuth");

        Assert.Equal("13.64.104.29", settings!.Host);
        Assert.Equal(389, settings.Port);
        Assert.False(settings.ValidateServerCertificate);
        Assert.False(settings.UseSSL);
        Assert.False(settings.AnonymousBind);
        Assert.Equal("mycompany\\binder", settings.BindUser);
        Assert.Equal("p@ssw0rd!", settings.BindPassword);
        Assert.Equal("ou=Lab,dc=mycompany,dc=local", settings.BaseDn);
        Assert.Equal("organizationalPerson", settings.ObjectClass);
        Assert.Equal("memberOf", settings.GroupMembershipAttribute);
        Assert.False(settings.RequireGroupMembership);
        Assert.Equal("RequiredGroupDn", settings.RequiredGroupDn);
        Assert.Equal(120, settings.PrincipalCacheDurationSec);
        Assert.Equal(new() {
                { "CN=ES-Accounting,CN=Users,DC=mycompany,DC=local", "accounting" },
                { "CN=ES-Operations,CN=Users,DC=mycompany,DC=local", "it" },
                { "CN=ES-Admins,CN=Users,DC=mycompany,DC=local", "$admins" }
            },
            settings.LdapGroupRoles);
    }
}