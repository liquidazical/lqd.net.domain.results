using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lqd.net.domain.results {

    public class UpdateResult<P> {

        public static UpdateResult<P> WasSuccess
                                       ( P value ) {

            return new Success( value );

        }

        public static UpdateResult<P> WasNotFound() {

            return new NotFound();
        }

        public static UpdateResult<P> WasError
                                        ( ResultError error ) {

            if( error == null ) throw new ArgumentNullException( nameof( error ) );

            return new Error( new [] { error } );
        }
 
        public static UpdateResult<P> WasError
                                       ( IEnumerable<ResultError> errors ) {

            return new Error( errors );
        }

        public UpdateResult<Q> Then<Q>
                                ( Func<P,Q> f ) {

            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return Match(
                success: value => UpdateResult<Q>.WasSuccess( f( value ) ),
                notFound: () => UpdateResult<Q>.WasNotFound(),
                error: errors => UpdateResult<Q>.WasError( errors )
            );

        }

        public void Match
                     ( Action<P> success
                     , Action notFound
                     , Action<IEnumerable<ResultError>> error ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( notFound == null ) throw new ArgumentNullException( nameof( notFound ) );
            if( error == null ) throw new ArgumentNullException( nameof( error ) );


            var unit = new object();


            Match(
              success: value => { success( value ); return unit; },
              notFound: () => { notFound(); return unit; },
              error: errors => { error( errors ); return unit; }
            );
        }

        public Q Match<Q>
                  ( Func<P,Q> success
                  , Func<Q> notFound
                  , Func<IEnumerable<ResultError>,Q> error ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( notFound == null ) throw new ArgumentNullException( nameof( notFound ) );
            if( error == null ) throw new ArgumentNullException( nameof( error ) );


            if( this is Success ) {
                return success( (this as Success).Value );

            } else if( this is NotFound ) {
                return notFound( );

            } else if( this is Error ) {
                return error( (this as Error).Errors );
            }
            throw new Exception( "Unexpected case" );
        }


        private class Success
                       : UpdateResult<P> {

            public Success
                    ( P value ) {

                if( value == null ) throw new ArgumentNullException( nameof( value ) );


                this.Value = value;

            }

            public readonly P Value;

        }

        private class NotFound
                       : UpdateResult<P> {}

        private class Error
                       : UpdateResult<P> {

            public Error
                    ( IEnumerable<ResultError> errors ) {

                if( !errors.Any() ) throw new ArgumentException( nameof( errors ) );


                this.Errors = errors;
            }


            public readonly IEnumerable<ResultError> Errors;
        }

        private UpdateResult() { }

    }

 
    public static class UpdateResultExtensions {

        
        public async static Task<UpdateResult<Q>> ThenAsync<P,Q>
                                                   ( this UpdateResult<P> task 
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => UpdateResult<Q>.WasSuccess(await f(value)),
                        notFound: () => Task.FromResult(UpdateResult<Q>.WasNotFound()),
                        error: errors => Task.FromResult(UpdateResult<Q>.WasError(errors))
                    );
        }


        public async static Task<UpdateResult<Q>> ThenAsync<P,Q>
                                                   ( this Task<UpdateResult<P>> task
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await (await task).ThenAsync( f );
        }

        public async static Task<UpdateResult<Q>> ThenAsync<P,Q>
                                                ( this Task<UpdateResult<P>> task 
                                                , Func<P,Q> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return (await task).Then( f );
        }
        
        public async static Task<UpdateResult<Q>> ThenAsync<Q>
                                                   ( this UpdateResult<Q> task 
                                                   , Func<Q,Task> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => { await f( value ); return task; } , 
                        notFound: () => Task.FromResult( UpdateResult<Q>.WasNotFound() ), 
                        error: errors => Task.FromResult( UpdateResult<Q>.WasError( errors  ) )
                    );
        }


        public static async Task<Q> MatchAsync<P,Q>
                                     ( this Task<UpdateResult<P>> task
                                     , Func<P,Q> success 
                                     , Func<Q> notFound
                                     , Func<IEnumerable<ResultError>,Q> error ) {


            return (await task).Match( success, notFound, error );
         
        }

        public static async void MatchAsync<P>
                                     ( this Task<UpdateResult<P>> task
                                     , Action<P> success
                                     , Action notFound
                                     , Action<IEnumerable<ResultError>> error ) {

            (await task).Match( success, notFound, error );
         
        }

    }
 
}
