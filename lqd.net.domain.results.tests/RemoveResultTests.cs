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

        // (done) then with an action that accepts no arguments will apply the action if it is a success
        // (done) then with an action that accepts no arguments will ignore the action if it is not a success
        // (done) then with an action that accepts no arguments returns the original value
        // (done) passing a null action to then with an action that accepts no arguments is invalid 



        [Fact]
        public void can_create_a_success_result() {

            var expected = new object();
            var result = RemoveResult<object>.WasSuccess( expected );

            result
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    notFound: () => Assert.False( true )                    
                );

        }

        [Fact]
        public void can_create_a_not_found_result() {

            var result = RemoveResult<object>.WasNotFound();

            result
                .Match(
                    success: value => Assert.False( true ),
                    notFound: () => Assert.True( true )
                    
                );

        }


        [Fact]
        public void passing_a_null_success_value_is_not_valid() {

            var act = (Action)( () => RemoveResult<object>.WasSuccess( null )  );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_success_function_to_match_is_invalid() {
            var unit = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( null, () => unit ));

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_not_found_action_to_match_is_invalid() {

            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => { }, null ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        [Fact]
        public void passing_a_null_not_found_function_to_match_is_invalid() {
            var unit = new object();
            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( () => result.Match( v => unit, null ));

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
                    notFound: () => Assert.False( true )
                );

        }

        [Fact]
        public void then_will_ignore_the_transformation_if_it_is_not_found() {

            var result = RemoveResult<object>.WasNotFound();

            result.Then<object>( v => { throw new Exception(); });
        }


        [Fact]
        public void passing_a_null_to_then_is_invalid() {

            var result = RemoveResult<object>.WasSuccess( new object() );
            var act = (Action)( ()  => result.Then<object>( null ) );


            Assert.Throws<ArgumentNullException>( act );
        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_apply_the_action_if_it_is_a_success() {

            var action_applied = false;
            var result = RemoveResult<object>.WasSuccess( new object() );


            result.Then( () => action_applied = true );


            Assert.True( action_applied );
        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_will_ignore_the_action_if_it_is_not_a_success() {

            var action_applied = false;
            var result = RemoveResult<object>.WasNotFound();

            result.Then( () => action_applied = true );


            Assert.False( action_applied );

        }

        [Fact]
        public void then_with_an_action_that_accepts_no_arguments_returns_the_original_value() {

            var expected = new object();
            var result = RemoveResult<object>.WasSuccess( expected );

            result
                .Then( () => { } )
                .Match(
                    success: value => Assert.Equal( expected, value ),
                    notFound: () => Assert.False( true )  
                );

        }

        [Fact]
        public void passing_a_null_action_to_then_with_an_action_that_accepts_no_arguments_is_invalid() {

            var action = (Action)null;
            var result = RemoveResult<object>.WasSuccess( new object() );

            var act = (Action)( () => result.Then( action ) );

            Assert.Throws<ArgumentNullException>( act );

        }

        public class RemoveError: ResultError { }

    }

}
