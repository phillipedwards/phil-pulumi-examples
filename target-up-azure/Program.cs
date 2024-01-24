using System.Collections.Generic;
using Pulumi;

return await Deployment.RunAsync(() =>
{
    /*
    Path to Reproduct the WTW issue:
    - Use this project
    - Comment the random resource out and ensure your storage account has a prompt or auto-created name
    - Run `pulumi up` and ensure the storage account is created
    - Uncomment the random resource and give the SA AccountName the Output.Format value which depends on the random resource
    - Run `pulumi up --target urn:pulumi:dev::target-up-azure::pulumi:providers:azure-native::azure` which should fail
    - The targeted up seems to be altering state of resources which ARE NOT specified in the target list which is a pretty bad bug IMO.
    - You should see an error message of  error: post-step event returned an error: failed to verify snapshot: resource urn:pulumi:dev::target-up-azure::azure-native:storage:StorageAccount::sa dependency urn:pulumi:dev::target-up-azure::random:index/randomString:RandomString::rand refers to missing resource
    - Also see the export_before and export_after files which shows the stack state before targeted up and after targeted up
    */
    var provider = new Pulumi.AzureNative.Provider("azure", new Pulumi.AzureNative.ProviderArgs());

    var rand = new Pulumi.Random.RandomString("rand", new Pulumi.Random.RandomStringArgs
    {
        Length = 6,
        Special = false,
        Upper = false
    });

    var rg = new Pulumi.AzureNative.Resources.ResourceGroup("rg", new()
    {
        Location = "WestUs",
    }, new CustomResourceOptions
    {
        Provider = provider
    });

    var sa = new Pulumi.AzureNative.Storage.StorageAccount("sa", new()
    {
        AccountName = Pulumi.Output.Format($"phil012424${rand.Result}"), // "phils012412154"
        Kind = Pulumi.AzureNative.Storage.Kind.StorageV2,
        ResourceGroupName = rg.Name,
        Sku = new Pulumi.AzureNative.Storage.Inputs.SkuArgs
        {
            Name = Pulumi.AzureNative.Storage.SkuName.Standard_LRS
        },
        Location = "WestUS",
    }, new CustomResourceOptions
    {
        Provider = provider
    });

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["outputKey"] = "outputValue"
    };
});
