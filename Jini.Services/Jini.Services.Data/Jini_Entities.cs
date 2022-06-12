using System.Data.Entity;

namespace Gyldendal.Jini.Services.Data
{
    public partial class Jini_Entities : DbContext
    {
        

        public Jini_Entities(string connectionString) : base(connectionString)
        {

        }
    }
}
