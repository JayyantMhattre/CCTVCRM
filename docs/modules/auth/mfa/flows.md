# MFA flows

Password login → MFA challenge (Redis, 5 min TTL) → MFA grant token issuance.

Failed attempts raise `MfaChallengeFailedDomainEvent` (audit).

Successful verification raises `MfaVerifiedDomainEvent`.
