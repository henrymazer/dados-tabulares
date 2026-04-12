# Auth: Middleware JWT

## Parent PRD

**PRD.md**

## What to build

Configurar autenticação JWT no projeto `Api`. O JWT é emitido pelo serviço de admin existente e validado localmente. O middleware deve extrair claims do token (tenant, módulos autorizados) e rejeitar requests sem token ou com token inválido/expirado. Incluir testes unitários para validação de token e extração de claims.

## Acceptance criteria

- [x] Middleware de autenticação JWT configurado no `Api`
- [x] Validação local do token (assinatura, expiração, issuer)
- [x] Claims extraídas: tenant ID, módulos autorizados
- [x] Request sem token retorna 401
- [x] Request com token expirado retorna 401
- [x] Request com token inválido retorna 401
- [x] Request com token válido mas sem acesso ao módulo retorna 403
- [x] Testes unitários cobrindo todos os cenários acima
- [x] Todos os testes passam

## Blocked by

- `work-items/01-setup-solution-infraestrutura-base.md`

## User stories addressed

- User story 19

## Type

AFK
