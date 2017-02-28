using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lqd.net.domain.results.tests {

    public class UpdateResultTests {

        // (done) can create a success result
        // (done) can create a not found result
        // (done) can create an error result for a single error 
        // (done) can create an error result for multiple errors 

        // (done) passing a null success value is invalid
        // (done) passing a null error is invalid
        // (done) passing an empty error is invalid

        // passing a null success to the match is invalid
        // passing a null notFound to the match is invalid
        // passing a null error to the match is invalid

        // (done) then with an action that accepts no arguments will apply the action if it is a success
        // (done) then with an action that accepts no arguments will ignore the action if it is not a success
        // (done) then with an action that accepts no arguments returns the original value
        // (done) passing a null action to then with an action that accepts no arguments is invalid 


        [Fact]
        public void can_create_a_success_result() {

            var expected = new object();
            var result = UpdateResult<object>.WasSuccess( expected );

            result
                .Match(  
                    success: value => Assert.Equal( expected, value ),
                    notFound: () => Assert.False( true ),
                    error: errs => Assert.False( true ) 
                );


        }

        [Fact]
        public void can_create_a_not_found_result() {

            var result = UpdateResult<object>.WasNotFound();


            result
                .Match( 
                    success: value => Assert.False( true ),
                    notFound: () => Assert.True( true ),
                    error: errs => Assert.False( true )  
                );
        }

        [Fact]
        public void can_create_an_error_result_for_a_single_error() {
            var error = new UpdateError();
            var result = UpdateResult<object>.WasError( error );


            result
                .Match(
                    success: value => Assert.False( true ),
                    notFound: () => Assert.False( true ),
                    error: errors => Assert.True( errors.Contains( error )  )
                );

        }

        [Fact]
        public void can_create_an_error_result_for_multiple_errors() {

            var errors = new [] { new UpdateError(), new UpdateError() };
            var result = UpdateResult<object>.WasError( errors );

            result
                .Match(
                    success: value => Assert.False( true ),
                    notFound: () => Assert.False( true ),
                    error: errs => Assert.Equal( errors, errs )
                );

        } 

        [Fact]
        public void passing_a_null_success_value_is_invalid() {

            var act = (Action)( () => UpdateResult<object>.WasSuccess( null ) );

            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_error_is_invalid() {
            var error = (UpdateError)null;
            var act = (Action)( () => UpdateResult<object>.WasError( error ) );

            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void passing_an_null_colleection_of_errors_is_invalid() {
            var errors =(IEnumerable<ResultError>)null; 
            var act = (Action)( () => UpdateResult<object>.WasError( errors )  );


            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_an_empty_error_is_invalid() {

            var act = (Action)( () => UpdateResult<object>.WasError( Enumerable.Empty<ResultError>()  ));

            Assert.Throws<ArgumentException>( act );

        }

        [Fact]
        public void passing_a_null_success_action_to_the_match_is_invalid() {

            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match(  null, () => { }, errs => { } ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_success_function_to_the_match_is_invalid() {
            var unit = new object();
            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match(  null, () => unit, errs => unit ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_not_found_action_to_the_match_is_invalid() {

            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( values => { }, null, errs => { } ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_not_found_function_to_the_match_is_invalid() {
            var unit = new object();
            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match(  value => unit, null, errs => unit ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_error_action_to_the_match_is_invalid() {

            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( values => { }, () => { }, null ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void passing_a_null_error_function_to_the_match_is_invalid() {
            var unit = new object();
            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match(  value => unit, () => unit, null ) );


            Assert.Throws<ArgumentNullException>( act );


        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_apply_the_action_if_it_is_a_success() {

            var action_applied = false;
            var result = UpdateResult<object>.WasSuccess( new object() );
            
            result.Then( () => action_applied = true );

            Assert.True( action_applied );


        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_ignore_the_action_if_it_is_not_a_success() {

            var action_applied = false;
            var result = UpdateResult<object>.WasError( new UpdateError() );

            result.Then( () => action_applied = true );

            Assert.False( action_applied );
        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_returns_the_original_value() {

            var expected = new object();
            var result = UpdateResult<object>.WasSuccess( expected );

            result
                .Then( () => { } )
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    notFound: () => Assert.False( true ),
                    error:  errs => Assert.False( true )  
                );

        }            

        [Fact]
        public void passing_a_null_action_to_then_with_an_action_that_accepts_no_arguments_is_invalid() {

            var action = (Action)null;
            var result = UpdateResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Then( action )  );
            

            Assert.Throws<ArgumentNullException>( act );

        }

        private class UpdateError : ResultError { }
    }
}
