using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lqd.net.domain.results {

    public class AddResult<P> {

        public static AddResult<P> WasSuccess
                                    ( P value ) {

            return new Success( value );
        }

        public static AddResult<P> WasError 
                                    ( ResultError error ) {

            if( error == null ) throw new ArgumentNullException( nameof( error ) );


            return new Error( new [] { error } );
        }

        public static AddResult<P> WasError
                                    ( IEnumerable<ResultError> errors ) {

            return new Error( errors  );
        }


        public AddResult<P> Then
                              ( Action<P> a ) {

            if( a == null ) throw new ArgumentNullException( nameof( a ) );


            return Then( value => {

                a( value );
                return value;
            });

        }

        public AddResult<Q> Then<Q>
                              ( Func<P,Q> f ) {

            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return Match(
                success: value => AddResult<Q>.WasSuccess( f( value ) ),
                error: errors => AddResult<Q>.WasError( errors )
            );

        }


        public void Match
                     ( Action<P> success
                     , Action<IEnumerable<ResultError>> error ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( error == null ) throw new ArgumentNullException( nameof( error ) );

            var unit = new object();

            Match(
                success: value => { success( value ); return unit; },
                error: errors => { error( errors ); return unit; }
            );

        }

        public Q Match<Q>
                  ( Func<P,Q> success 
                  , Func<IEnumerable<ResultError>,Q> error ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( error == null ) throw new ArgumentNullException( nameof( error ) );

            if( this is Success ) {
                return success( (this as Success).Value );

            } else if( this is Error ) {
                return error( (this as Error).Errors );

            }
            throw new Exception( "Unexpected case" );
        }

        private class Success
                       : AddResult<P> {

            public Success
                    ( P value ) {

                if( value == null ) throw new ArgumentNullException( nameof( value ) );

                this.Value = value;

            }


            public readonly P Value;
        }

        private class Error 
                       : AddResult<P> {

            public Error
                    ( IEnumerable<ResultError> errors ) {

                if( errors == null ) throw new ArgumentNullException( nameof( errors ) );
                if( !errors.Any() ) throw new ArgumentException( nameof( errors ) );


                this.Errors = errors;
            }


            public readonly IEnumerable<ResultError> Errors;

        }

        private AddResult() { }
    }


    public static class AddResultExtensions {


        public async static Task<AddResult<Q>> ThenAsync<P,Q>
                                                ( this AddResult<P> task 
                                                , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => AddResult<Q>.WasSuccess( await f( value ) ),
                        error: errors => Task.FromResult( AddResult<Q>.WasError( errors  ) ) 
                    );
        }


        public async static Task<AddResult<Q>> ThenAsync<P,Q>
                                                ( this Task<AddResult<P>> task
                                                , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await (await task).ThenAsync( f );
        }

        public async static Task<AddResult<Q>> ThenAsync<P,Q>
                                                ( this Task<AddResult<P>> task 
                                                , Func<P,Q> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return (await task).Then( f );
        }

        public async static Task<AddResult<Q>> ThenAsync<Q>
                                                ( this AddResult<Q> task 
                                                , Func<Q,Task> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => { await f( value ); return task; } , 
                        error: errors => Task.FromResult( AddResult<Q>.WasError( errors  ) )
                    );
        }


        public static async Task<Q> MatchAsync<P,Q>
                                     ( this Task<AddResult<P>> task
                                     , Func<P,Q> success 
                                     , Func<IEnumerable<ResultError>,Q> error ) {


            return (await task).Match( success, error );
         
        }

        public static async void MatchAsync<P>
                                     ( this Task<AddResult<P>> task
                                     , Action<P> success
                                     , Action<IEnumerable<ResultError>> error ) {

            (await task).Match( success, error );
         
        }

    }

}
