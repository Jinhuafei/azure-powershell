// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.ContainerRegistry
{
    [Cmdlet(VerbsCommon.Get, ContainerRegistryReplicationNoun,
        DefaultParameterSetName = NameResourceGroupParameterSet,
        SupportsShouldProcess = true), OutputType(typeof(PSContainerRegistryReplication))]
    public class GetAzureContainerRegistryReplication : ContainerRegistryCmdletBase
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ParameterSetName = NameResourceGroupParameterSet,
            HelpMessage = "Resource Group Name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = NameResourceGroupParameterSet,
            HelpMessage = "Container Registry Name.")]
        [Alias(ContainerRegistryNameAlias, RegistryNameAlias, ResourceNameAlias)]
        [ValidateNotNullOrEmpty]
        public string RegistryName { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = RegistryObjectParameterSet,
            ValueFromPipeline = true,
            HelpMessage = "Container Registry Object.")]
        [ValidateNotNullOrEmpty]
        public PSContainerRegistry Registry { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Container Registry Replication Name.")]
        [ValidateNotNullOrEmpty]
        [Alias(ReplicationNameAlias)]
        public string Name { get; set; }
        
        public override void ExecuteCmdlet()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                WriteObject(new PSContainerRegistryReplication());
            }
            else
            {
                var replications = new List<PSContainerRegistryReplication>();

                WriteObject(replications);
            }
        }
    }
}
