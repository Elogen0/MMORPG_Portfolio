using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer
{
    public enum ServerErrorCode
    {
        NONE = 0,
        NOT_SUFFICIENT_INVENTORY = 1,
        DB_TRANSACTION_FAILED = 2,
    }
}
