using IdentitySample.ViewModels.Role;
using System.Collections.Generic;

namespace IdentitySample.Repositories
{
    public interface IUtilities
    {
        public IList<ActionAndControllerName> AreaAndActionAndControllerNamesList();
        public IList<string> GetAllAreasNames();
        public string DataBaseRoleValidationGuid();
    }

}
