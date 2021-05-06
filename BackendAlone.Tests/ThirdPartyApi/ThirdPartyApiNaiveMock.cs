using System;
using System.Collections.Generic;
using System.Linq;
using ThirdPartyApi.Api;
using ThirdPartyApi.Model;

namespace BackendAlone.Tests.ThirdPartyApi
{
    public class ThirdPartyApiNaiveMock : IThirdPartyApi
    {
        private IList<Path> _travelsDatabase = new List<Path>();

        public IList<Path> GetPaths()
        {
            return _travelsDatabase;
        }
        public void AddPath(Path path)
        {
            // instance is added
            _travelsDatabase.Add(path);
        }
    }
}
