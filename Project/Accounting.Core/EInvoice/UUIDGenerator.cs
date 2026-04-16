using System;

namespace Accounting.Core.EInvoice
{
    public static class UUIDGenerator
    {
        public static string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}