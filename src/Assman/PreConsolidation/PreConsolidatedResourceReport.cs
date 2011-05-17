using System.Collections.Generic;

namespace Assman.PreConsolidation
{
    public class PreConsolidatedResourceReport
    {
        public List<PreConsolidatedResourceGroup> Groups { get; set; }

        public List<PreCompiledSingleResource> SingleResources { get; set; }

        public PreConsolidatedResourceReport()
        {
            Groups = new List<PreConsolidatedResourceGroup>();
            SingleResources = new List<PreCompiledSingleResource>();
        }
    }
}