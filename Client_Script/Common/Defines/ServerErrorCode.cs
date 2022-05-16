using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame
{
    public enum ServerErrorCode
    {
        NONE = 0,
        NOT_SUFFICIENT_INVENTORY = 1,
        DB_TRANSACTION_FAILED = 2,
    }
}
