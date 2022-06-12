using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Contracts
{
    public enum ProductStatus
    {
        [Description("Konfigureret")]
        Configured = 1,
        [Description("Afventer opsætning")]
        AwaitingSetup = 2,
        [Description("Afventer salgsopsætning")]
        AwaitingSalesSetup = 3,
        [Description("Afventer loginopsætning")]
        PendingLoginSetup = 4
    }
}
