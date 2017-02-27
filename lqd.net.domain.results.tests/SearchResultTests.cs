using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lqd.net.domain.results.tests {

    public class SearchResultTests {

        // (done) can create an ok result
        // (done) can create a bad request result with a single error
        // (done) can create a bad request result with multiple errors

        // (done) passing a null value into WasOk is invalid
        // (done) passing a null value into WasBadRequest is invalid
        // (done) passing a null collection of errors into WasBadRequest is invalid
        // (done) passing an empty collection of errors into WasBadRequest is invalid

        // (done) passing a null ok action into match is invalid 
        // (done) passing a null bad request action into match is invalid
        // (done) passing a null ok function into match is invalid
        // (done) passing a null bad request function into match in invalid
        
        // (done) then applies the transformation if it is an ok result
        // (done) then ignores the transformation if it is a bad request result
        // passing a null into the is invalid


        [Fact]
        public void can_create_an_ok_result() {
            var expected = new object();
            var result = SearchResult<object>.WasOk( expected );


            result
                .Match( 
                    ok: value => Assert.Equal( expected, value ),
                    badRequest: errors => Assert.False( true )
                );
        }

        [Fact]
        public void can_create_a_bad_request_result_with_a_single_error() {
            var error = new SearchError();
            var result = SearchResult<object>.WasBadRequest( error  );

            result
                .Match(
                  ok: value => Assert.False( true ),
                  badRequest: errors => Assert.True( errors.Contains( error ) )    
                );
        }

        [Fact]
        public void can_create_a_bad_request_result_with_multiple_errors() {
            var errors = new [] { new SearchError(), new SearchError() };
            var result = SearchResult<object>.WasBadRequest( errors );

            result
                .Match(
                    ok: value => Assert.False( true ),
                    badRequest: errs => Assert.Equal( errors, errs )  
                );

        }

        [Fact]
        public void passing_a_null_value_into_WasOk_is_invalid() {

            var act = (Action)( () => SearchResult<object>.WasOk( null ) );


            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_value_into_WasBadRequest_is_invalid() {

            var error = (ResultError)null;
            var act = (Action)( () => SearchResult<object>.WasBadRequest( error ) );  


            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_collection_of_errors_into_WasBadRequest_is_invalid() {
            var error = (IEnumerable<ResultError>)null;
            var act = (Action)( () => SearchResult<object>.WasBadRequest( error ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_an_empty_collection_of_errors_into_WasBadRequest_is_invalid() {
            var errors = Enumerable.Empty<ResultError>();
            var act = (Action)( () => SearchResult<object>.WasBadRequest( errors ) );


            Assert.Throws<ArgumentException>( act );
        }

        [Fact]
        public void passing_a_null_ok_action_into_match_is_invalid() {

            var result = SearchResult<object>.WasOk( new object() );
            var act = (Action)( () => result.Match( null, e => { } ));


            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_bad_request_action_into_match_is_invalid() {

            var result = SearchResult<object>.WasOk( new object() );
            var act = (Action)( () => result.Match( v => { }, null ) );

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_ok_function_into_match_is_invalid() {
            var unit = new object();
            var result = SearchResult<object>.WasOk( new object() );
            var act = (Action)( () => result.Match( null, e => unit ));


            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_bad_function_action_into_match_is_invalid() {
            var unit = new object();
            var result = SearchResult<object>.WasOk( new object() );
            var act = (Action)( () => result.Match( v => unit, null ) );

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void then_applies_the_transformation_if_it_is_an_ok_result() {

            var expected = new object();
            var result = SearchResult<object>.WasOk( new object() );


            result
                .Then( v => expected )
                .Match(
                    ok: value => Assert.Equal( expected, value ),
                    badRequest: errors => Assert.False( true )  
                );

        }

        [Fact]
        public void then_ignores_the_transformation_if_it_is_a_bad_request_result() {

            var result = SearchResult<object>.WasBadRequest( new SearchError() );

            result.Then<object>( v => { throw new Exception(); } );
        }

        [Fact]
        public void passing_a_null_into_the_is_invalid() {

            var result = SearchResult<object>.WasOk( new object() );
            var act = (Action)( () => result.Then<object>( null ) );


            Assert.Throws<ArgumentNullException>( act );
        }


        public class SearchError : ResultError { }
    }
}
