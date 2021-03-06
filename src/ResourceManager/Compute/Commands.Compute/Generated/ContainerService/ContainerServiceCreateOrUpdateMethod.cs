//
// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

// Warning: This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using Microsoft.Azure.Commands.Compute.Automation.Models;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Compute.Automation
{
    public partial class InvokeAzureComputeMethodCmdlet : ComputeAutomationBaseCmdlet
    {
        protected object CreateContainerServiceCreateOrUpdateDynamicParameters()
        {
            dynamicParameters = new RuntimeDefinedParameterDictionary();
            var pResourceGroupName = new RuntimeDefinedParameter();
            pResourceGroupName.Name = "ResourceGroupName";
            pResourceGroupName.ParameterType = typeof(string);
            pResourceGroupName.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 1,
                Mandatory = true
            });
            pResourceGroupName.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ResourceGroupName", pResourceGroupName);

            var pContainerServiceName = new RuntimeDefinedParameter();
            pContainerServiceName.Name = "Name";
            pContainerServiceName.ParameterType = typeof(string);
            pContainerServiceName.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 2,
                Mandatory = true
            });
            pContainerServiceName.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("Name", pContainerServiceName);

            var pParameters = new RuntimeDefinedParameter();
            pParameters.Name = "ContainerService";
            pParameters.ParameterType = typeof(ContainerService);
            pParameters.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 3,
                Mandatory = true
            });
            pParameters.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ContainerService", pParameters);

            var pArgumentList = new RuntimeDefinedParameter();
            pArgumentList.Name = "ArgumentList";
            pArgumentList.ParameterType = typeof(object[]);
            pArgumentList.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByStaticParameters",
                Position = 4,
                Mandatory = true
            });
            pArgumentList.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ArgumentList", pArgumentList);

            return dynamicParameters;
        }

        protected void ExecuteContainerServiceCreateOrUpdateMethod(object[] invokeMethodInputParameters)
        {
            string resourceGroupName = (string)ParseParameter(invokeMethodInputParameters[0]);
            string containerServiceName = (string)ParseParameter(invokeMethodInputParameters[1]);
            ContainerService parameters = (ContainerService)ParseParameter(invokeMethodInputParameters[2]);

            var result = ContainerServicesClient.CreateOrUpdate(resourceGroupName, containerServiceName, parameters);
            WriteObject(result);
        }
    }

    public partial class NewAzureComputeArgumentListCmdlet : ComputeAutomationBaseCmdlet
    {
        protected PSArgument[] CreateContainerServiceCreateOrUpdateParameters()
        {
            string resourceGroupName = string.Empty;
            string containerServiceName = string.Empty;
            ContainerService parameters = new ContainerService();

            return ConvertFromObjectsToArguments(
                 new string[] { "ResourceGroupName", "ContainerServiceName", "Parameters" },
                 new object[] { resourceGroupName, containerServiceName, parameters });
        }
    }

    [Cmdlet(VerbsCommon.New, "AzureRmContainerService", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSContainerService))]
    public partial class NewAzureRmContainerService : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.Name, VerbsCommon.New))
                {
                    string resourceGroupName = this.ResourceGroupName;
                    string containerServiceName = this.Name;
                    ContainerService parameters = new ContainerService();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<PSContainerService, ContainerService>(this.ContainerService, parameters);

                    var result = ContainerServicesClient.CreateOrUpdate(resourceGroupName, containerServiceName, parameters);
                    var psObject = new PSContainerService();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<ContainerService, PSContainerService>(result, psObject);
                    WriteObject(psObject);
                }
            });
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ResourceManager.Common.ArgumentCompleters.ResourceGroupCompleter()]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 3,
            Mandatory = true,
            ValueFromPipeline = true)]
        public PSContainerService ContainerService { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }
    }

    [Cmdlet(VerbsData.Update, "AzureRmContainerService", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSContainerService))]
    public partial class UpdateAzureRmContainerService : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.Name, VerbsData.Update))
                {
                    string resourceGroupName = this.ResourceGroupName;
                    string containerServiceName = this.Name;
                    ContainerService parameters = new ContainerService();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<PSContainerService, ContainerService>(this.ContainerService, parameters);

                    var result = ContainerServicesClient.CreateOrUpdate(resourceGroupName, containerServiceName, parameters);
                    var psObject = new PSContainerService();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<ContainerService, PSContainerService>(result, psObject);
                    WriteObject(psObject);
                }
            });
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ResourceManager.Common.ArgumentCompleters.ResourceGroupCompleter()]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 3,
            Mandatory = true,
            ValueFromPipeline = true)]
        public PSContainerService ContainerService { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }
    }
}
