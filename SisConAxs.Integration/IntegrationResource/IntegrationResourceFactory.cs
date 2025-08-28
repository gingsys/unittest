using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public class IntegrationResourceFactory
    {
        public static IIntegrationResource Get(int ID)
        {
            if (ID == IntegrationResourceOracle.ACTION_ID)
            {
                return (IIntegrationResource)new IntegrationResourceOracle();
            }
            //else if (ID == IntegrationResourceSRA.ACTION_ID)
            //{
            //    return (IIntegrationResource)new IntegrationResourceSRA();
            //}
            return null;
        }

        public static IIntegrationIcarusAccess GetIcarusAccess(int ID)
        {
            if (ID == IntegrationResourceIcarusAccess.ACTION_ID)
            {
                return (IIntegrationIcarusAccess)new IntegrationResourceIcarusAccess();
            }

            return null;
        }
    }
}
