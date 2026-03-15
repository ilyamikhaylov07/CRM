namespace Crm.Infrastructure.Keycloak.Models;


/// <summary>
/// Минимальное представление realm-роли Keycloak для назначения пользователю.
/// </summary>
public sealed class KeycloakRoleRepresentation
{
    public required string Id { get; init; }

    public required string Name { get; init; }
}
