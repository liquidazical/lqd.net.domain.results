using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lqd.net.domain.results.tests {

    public class RemoveResultTests {

        // (done) can create a success result
        // (done) can create a not found result
        // (done) can create an error result for a single error
        // (done) can create an error result for multiple errors
        
        // (done) passing a null success value is not valid
        // (done) passing a null error is not valid
        // (done) passing a null collection of errors is not valid
        // (done) passing an empty collection of errors is not valid

        // (done) passing a null success action to match is invalid
        // (done) passing a null success function to match is invalid
        // (done) passing a null not found action to match is invalid
        // (done) passing a null not found function to match is invalid
        // (done) passing a null error action to match in invalid
        // (done) passing a null error function to match is invalid

        // (done) then will apply the transformation if it is a success
        // (done) then will ignore the transformation if it is not found
        // (done) then will ignore the transformation if it is an error
        // passing a null to then is invalid


        [Fact]
        public void can_create_a_success_result() {

            var expected = new object();
            var result = RemoveResult<object>.WasSuccess( expected );

            result
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    notFound: () => Assert.False( true ),
                    error: errors => Assert.False( true )
                );

        }

        [Fact]
        public void can_create_a_not_found_result() {

            var result = RemoveResult<object>.WasNotFound();

            result
                .Match(
                    success: value => Assert.False( true ),
                    notFound: () => Assert.True( true ),
                    error: errors => Assert.False( true )
                );

        }

        [Fact]
        public void can_create_an_error_result_for_a_single_error() {
            var error = new RemoveError();
            var result = RemoveResult<object>.WasError( error );

            result
                .Match( 
                    success: value => Assert.False( true ),
                    notFound: () => Assert.False( true ),
                    error: errors => Assert.True( errors.Contains( error ) )   
                );

        }

        [Fact]
        public void can_create_an_error_from_multiple_errors() {

            var errors = new [] { new RemoveError(), new RemoveError() };
            var result = RemoveResult<object>.WasError( errors );


            result
                .Match(
                    success: value => Assert.False( true ),
                    notFound: () => Assert.False( true ) ,
                    error: errs => Assert.Equal( errors, errs )
                );

        }

        [Fact]
        public void passing_a_null_success_value_is_not_valid() {

            var act = (Action)( () => RemoveResult<object>.WasSuccess( null )  );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_error_is_not_valid () {

            var error = (ResultError)null;
            var act = (Action)( () => RemoveResult<object>.WasError( error ) );

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_collection_of_errors_is_not_valid() {
            var errors = (IEnumerable<ResultError>)null;
            var act = (Action)( () => RemoveResult<object>.WasError( errors ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_an_empty_collection_of_errors_is_not_valid() {

            var errors = Enumerable.Empty<ResultError>();
            var act = (Action)( () => RemoveResult<object>.WasError( errors ) );

            Assert.Throws<ArgumentException>( act );


        }

        [Fact]
        public void passing_a_null_success_action_to_match_is_invalid() {
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( null, () => { }, errs => { } ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_success_function_to_match_is_invalid() {
            var unit = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( null, () => unit, errs => unit ));

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_not_found_action_to_match_is_invalid() {

            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => { }, null, e => { } ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_not_found_function_to_match_is_invalid() {
            var unit = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => unit, null, e => unit ));

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_error_action_to_match_in_invalid() {

            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => { }, () => { }, null ) ); 

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_a_null_error_function_to_match_is_invalid() {

            var unit = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => unit, () => unit, null ) );

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void then_will_apply_the_transformation_if_it_is_a_success() {
            var expected = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );

            result
                .Then( v => expected )
                .Match(  
                    success: value => Assert.Equal( expected, value ), 
                    notFound: () => Assert.False( true ),
                    error: errors => Assert.False( true )
                );

        }

        [Fact]
        public void then_will_ignore_the_transformation_if_it_is_not_found() {

            var result = RemoveResult<object>.WasNotFound();

            result.Then<object>( v => { throw new Exception(); });
        }

        [Fact]
        public void then_will_ignore_the_transformation_if_it_is_an_error() {

            var result = RemoveResult<object>.WasError( new ResultError() );

            result.Then<object>( v => { throw new Exception(); });

        }

        [Fact]
        public void passing_a_null_to_then_is_invalid() {

            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( ()  => result.Then<object>( null ) );


            Assert.Throws<ArgumentNullException>( act );
        }


        public class RemoveError: ResultError { }

    }

}
