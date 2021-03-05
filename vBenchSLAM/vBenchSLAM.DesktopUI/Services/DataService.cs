using System;
using System.Collections;
using System.Collections.Generic;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.Services
{
    public class DataService : IDataService
    {
        public IEnumerable<FrameworkModel> GetAvailableFrameworks()
        {
            var retVals = new List<FrameworkModel>();
            foreach (MapperTypeEnum value in Enum.GetValues(typeof(MapperTypeEnum)))
            {
                retVals.Add(new FrameworkModel(){Name = value.GetStringValue(), Id = (int)value});
            }

            return retVals;
        }
    }
}