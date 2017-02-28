using System.Collections.Generic;
using System.Linq;

namespace lqd.net.domain.results {

    // Representation of an error. Note this is deliberately not an exception
    // to highlight the difference between an exceptional case ( Exception ) and
    // an anticipated error  
    public class ResultError {}


    public static class ResultErrorExtensions {

        public static bool Has<T>
                            ( this IEnumerable<ResultError> errors ) {

            return errors
                    .OfType<T>()
                    .Any();

        }

    }
}
