using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lqd.net.domain.results {

    /// Represents the result of a remove operation that removes 
    /// an item by Id
    public class RemoveResult<P> {

        public static RemoveResult<P> WasSuccess
                                       ( P value ) {

            return new Success( value );
        }

        public static RemoveResult<P> WasNotFound() {

            return new NotFound();
        }


        public RemoveResult<P> Then
                                ( Action a ) {

            if( a == null ) throw new ArgumentNullException( nameof( a ) );


            return Then( value => {
                a();
                return value;
            });

        }

        public RemoveResult<P> Then
                                ( Action<P> a ) {

            if( a == null ) throw new ArgumentNullException( nameof( a ) );


            return Then( value => {
                a( value );
                return value;
            });

        }



        public RemoveResult<Q> Then<Q>
                                ( Func<P,Q> f ) {

            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return Match(
                success: value => RemoveResult<Q>.WasSuccess( f( value ) ),
                notFound: () => RemoveResult<Q>.WasNotFound()
            );
        }


        public void Match
                     ( Action<P> success 
                     , Action notFound ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( notFound == null ) throw new ArgumentNullException( nameof( notFound ));


            var unit = new object();

            Match(
                success: value => { success( value ); return unit; },
                notFound: () => { notFound(); return unit; }
            );
        }

        public Q Match<Q>
                  ( Func<P,Q> success
                  , Func<Q> notFound ) {

            if( success == null ) throw new ArgumentNullException( nameof( success ) );
            if( notFound == null ) throw new ArgumentNullException( nameof( notFound ) );


            if( this is Success ) {
                return success( (this as Success).Value );

            } else if( this is NotFound ) {
                return notFound( );

            }
            throw new Exception( "Unexpected case" );
        }

        private class Success
                       : RemoveResult<P> {

            public Success
                    ( P value ) {

                if( value == null ) throw new ArgumentNullException( nameof( value ));

                this.Value = value;
            }

            public readonly P Value;

        }

        private class NotFound
                       : RemoveResult<P> {}

        private RemoveResult() {}

    }


    public static class RemoveResultExtensions {

        public async static Task<RemoveResult<Q>> ThenAsync<P,Q>
                                                   ( this RemoveResult<P> task 
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => RemoveResult<Q>.WasSuccess( await f( value )),
                        notFound: () => Task.FromResult( RemoveResult<Q>.WasNotFound() )                        
                    );
        }

        public async static Task<RemoveResult<Q>> ThenAsync<P,Q>
                                                   ( this Task<RemoveResult<P>> task 
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await (await task).ThenAsync( f );
                
        }

        public async static Task<RemoveResult<Q>> ThenAsync<P,Q>
                                                   ( this Task<RemoveResult<P>> task 
                                                   , Func<P,Q> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return (await task).Then( f );
                
        }

        public async static Task<RemoveResult<Q>> ThenAsync<Q>
                                                    ( this RemoveResult<Q> task 
                                                    , Func<Q,Task> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        success: async value => { await f( value ); return task; } , 
                        notFound: () => Task.FromResult( RemoveResult<Q>.WasNotFound() )                        
                    );
        }




        public static async Task<Q> MatchAsync<P,Q>
                                     ( this Task<RemoveResult<P>> task
                                     , Func<P,Q> success 
                                     , Func<Q> notFound ) {

            return (await task).Match( success, notFound );
         
        }


        public static async void MatchAsync<P>
                                     ( this Task<RemoveResult<P>> task
                                     , Action<P> success
                                     , Action notFound ) {

            (await task).Match( success, notFound );
         
        }



    }
}
