using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataSample
{
    public class SpaceStudyService : Service
    {
        public virtual SpaceStudy GetSpaceStudy(int id)
        {
            return new SpaceStudy
            {
                Id = id,
                Name = "UNMOCKED"
            };
        }

    }
}
