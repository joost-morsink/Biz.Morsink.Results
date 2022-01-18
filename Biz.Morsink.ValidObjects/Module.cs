global using System; 
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using Biz.Morsink.Results;
global using Biz.Morsink.Results.Errors;
global using static Biz.Morsink.ValidObjects.Statics;
global using Biz.Morsink.ValidObjects.Constraints;
global using Biz.Morsink.ValidObjects.Math;

namespace Biz.Morsink.ValidObjects;
public static class Statics
{
    public static Result.ForErrorType<ErrorList> R = default;
}
