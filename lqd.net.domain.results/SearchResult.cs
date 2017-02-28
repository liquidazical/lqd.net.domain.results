using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lqd.net.domain.results {

    /// Represents the result of a seach operation as opposed to
    /// a finding a single item by its Id
    public class SearchResult<P> {

        public static SearchResult<P> WasOk
                                       ( P value ) {

            return new Ok( value  );
        }

        public static SearchResult<P> WasBadRequest
                                       ( ResultError error ) {

            if( error == null ) throw new ArgumentNullException( nameof( error ) );


            return new BadRequest( new [] { error } );
        }

        public static SearchResult<P> WasBadRequest
                                       ( IEnumerable<ResultError> errors ) {

            return new BadRequest( errors );

        }


        public SearchResult<P> Then
                                ( Action a ) {

            if( a == null ) throw new ArgumentNullException( nameof( a ) );


            return Then( value => {
                a();
                return value;
            });

        }

        public SearchResult<P> Then
                                ( Action<P> a ) {

            if( a == null ) throw new ArgumentNullException( nameof( a ) );


            return Then( value => {
                a( value );
                return value;
            });

        }

        public SearchResult<Q> Then<Q>
                                ( Func<P,Q> f ) {

            if( f == null ) throw new ArgumentNullException( nameof( f ) );

            return Match(
                ok: value => SearchResult<Q>.WasOk( f( value )  ),
                badRequest: errors => SearchResult<Q>.WasBadRequest( errors )
            );

        }

        public void Match
                     ( Action<P> ok 
                     , Action<IEnumerable<ResultError>> badRequest ) {

            if( ok == null ) throw new ArgumentNullException( nameof( ok ) );
            if( badRequest == null ) throw new ArgumentNullException( nameof( badRequest ) );


            var unit = new object();

            Match(
              ok: value => { ok( value ); return unit; },
              badRequest: errors => { badRequest( errors ); return unit; }
            );
        }

        public Q Match<Q>
                  ( Func<P,Q> ok
                  , Func<IEnumerable<ResultError>,Q> badRequest ) {

            if( ok == null ) throw new ArgumentNullException( nameof( ok ) );
            if( badRequest == null ) throw new ArgumentNullException( nameof( badRequest ) );


            if( this is Ok ) {
                return ok( (this as Ok).Value );

            } else if( this is BadRequest ) {
                return badRequest( (this as BadRequest).Errors );

            }
            throw new Exception( "Unexpected case" );
        }


        private class Ok
                       : SearchResult<P> {

            public Ok
                    ( P value ) {

                if( value == null ) throw new ArgumentNullException( nameof( value ) );


                this.Value = value;
            }


            public readonly P Value;
        }


        private class BadRequest
                       : SearchResult<P> {

            public BadRequest
                    ( IEnumerable<ResultError> errors ) {

                if( errors == null ) throw new ArgumentNullException( nameof( errors ) );
                if( !errors.Any() ) throw new ArgumentException( nameof( errors ) );

                this.Errors = errors;
            }


            public readonly IEnumerable<ResultError> Errors;

        }

        private SearchResult() {}
    }


    public static class SearchResultExtensions {

        public async static Task<SearchResult<Q>> ThenAsync<P,Q>
                                                   ( this SearchResult<P> task 
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await
                task
                    .Match(
                        ok: async value => SearchResult<Q>.WasOk( await f( value )),
                        badRequest: errors => Task.FromResult( SearchResult<Q>.WasBadRequest( errors ))
                    );
        }

        public async static Task<SearchResult<Q>> ThenAsync<P,Q>
                                                   ( this Task<SearchResult<P>> task 
                                                   , Func<P,Task<Q>> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return await (await task).ThenAsync( f );
                
        }

        public async static Task<SearchResult<Q>> ThenAsync<P,Q>
                                                   ( this Task<SearchResult<P>> task 
                                                   , Func<P,Q> f ) {

            if( task == null ) throw new ArgumentNullException( nameof( task ) );
            if( f == null ) throw new ArgumentNullException( nameof( f ) );


            return (await task).Then( f );
                
        }



        public static async Task<Q> MatchAsync<P,Q>
                                     ( this Task<SearchResult<P>> task
                                     , Func<P,Q> ok 
                                     , Func<IEnumerable<ResultError>,Q> badRequest ) {

            return (await task).Match( ok, badRequest );
         
        }


        public static async void MatchAsync<P>
                                     ( this Task<SearchResult<P>> task
                                     , Action<P> ok
                                     , Action<IEnumerable<ResultError>> badRequest ) {

            (await task).Match( ok, badRequest );
         
        }




    }
}
