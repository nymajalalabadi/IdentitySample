using IdentitySample.ViewModels.Role;
using System.Collections.Generic;

namespace IdentitySample.Repositories
{
    public interface IUtilities
    {
        public IList<ActionAndControllerName> ActionAndControllerNamesList();
        public IList<string> GetAllAreasNames();
        public string DataBaseRoleValidationGuid();
    }

}
