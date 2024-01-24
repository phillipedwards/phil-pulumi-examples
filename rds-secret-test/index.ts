import * as aws from "@pulumi/aws";

const test = new aws.rds.Cluster("test", {
    engine: "aurora-mysql",
    clusterIdentifier: "example",
    databaseName: "test",
    manageMasterUserPassword: true,
    masterUsername: "admin",
    skipFinalSnapshot: true,
});

// NOTE: this example assumes the masterUserSecrets array will ALWAYS have a single instance in it.
// lookup our the created secret and its current version, then export the needed value(s).
const secret = aws.secretsmanager.getSecretOutput({
    arn: test.masterUserSecrets.apply(masterUserSecrets => masterUserSecrets[0].secretArn),
});

const secretVersion = aws.secretsmanager.getSecretVersionOutput({
    secretId: secret.id,
});

export const password = secretVersion.secretString;