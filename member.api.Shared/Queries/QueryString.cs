using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace member.api.Shared.Queries
{
    public class QueryString
    {
        public static readonly string GetPassword = @"select
                                                        a.password
                                                    from user_authens a
                                                    where a.user_name = @UserName";
    }
}