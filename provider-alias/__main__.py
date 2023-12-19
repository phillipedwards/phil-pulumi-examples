"""An AWS Python Pulumi program"""

import pulumi
from pulumi_aws import (
    ssm,
    Provider
)

cfg = pulumi.Config("aws")
# NOTE: you will see some default values be updated so be aware and handle as you see fit
provider = Provider(
    "provider",
    region=cfg.require("region"),
    profile=cfg.require("profile"),
    # use the URN of the old default provider to stop resource re-creation
    opts=pulumi.ResourceOptions(aliases=["urn:pulumi:phil::provider-alias::pulumi:providers:aws::default_5_30_1"])
)

# Create an AWS resource (ssm param)
param = ssm.Parameter(
    "myparam",
    value="some value",
    type="String",
    opts=pulumi.ResourceOptions(provider=provider)
)

# Export the name of the bucket
pulumi.export('ssm_id', param.id)
