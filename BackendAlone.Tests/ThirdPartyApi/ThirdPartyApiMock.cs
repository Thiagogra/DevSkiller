using System;
using System.Collections.Generic;
using System.Linq;
using ThirdPartyApi.Api;
using ThirdPartyApi.Model;

namespace BackendAlone.Tests.ThirdPartyApi
{
    public class ThirdPartyApiMock : IThirdPartyApi
    {
        private IList<Path> _travelsDatabase = new List<Path>();

        public IList<Path> GetPaths()
        {
            return _travelsDatabase;
        }
        public void AddPath(Path path)
        {
            var existingPaths = _travelsDatabase;
            foreach (var toPath in path.From)
            {
                var existingPath = existingPaths.FirstOrDefault(x => x.Name == toPath.Name);
                if (existingPath != null)
                {
                    existingPath.From.Add(path);
                }
                else
                {
                    throw new Exception($"Parent path to path {path.Name} were not added yet.");
                }
            }
            // copy is added
            _travelsDatabase.Add(new Path() { Name = path.Name });
        }
    }
}
