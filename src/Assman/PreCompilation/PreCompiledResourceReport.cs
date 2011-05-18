using System;
using System.Collections.Generic;

namespace Assman.PreCompilation
{
    public class PreCompiledResourceReport
    {
        public List<PreCompiledResourceGroup> Groups { get; set; }

        public List<PreCompiledSingleResource> SingleResources { get; set; }

        public PreCompiledResourceReport()
        {
            Groups = new List<PreCompiledResourceGroup>();
            SingleResources = new List<PreCompiledSingleResource>();
        }
    }
}