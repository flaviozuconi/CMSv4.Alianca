using System;

/// <summary>
/// Lista de permissões utilizadas por grupos / funcionalidades
/// </summary>
public enum Permissao
{
    Publico = 0,
    Visualizar = 1,
    Modificar = 2,
    Excluir = 3
}

/// <summary>
/// Lista de permissões utilizadas para as páginas da área pública
/// </summary>
public enum PermissaoPublica
{
    AcessoNegado = 0,
    NaoLogado = 1,
    Logado = 2,
    AcessoLiberado = 3
}
