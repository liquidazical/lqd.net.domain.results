# lqd.net.dmomain.results

Is a .net assembly that contains result types that may be returned from a domain layer. There is the following result types:

* AddResult
* RemoveResult
* SearchResult
* UpdateResult

The idea is that success and error paths are identified by the different outcomes supported by a result, this leaves exceptions to be just treat as exceptional cases i.e. an exception is an internal server error.

**Note**: The authorisaztion failure outcome is currently not implemented yet.


### AddResult

Is a result returned from a domain command that creates an entity.  It supports the following outcomes:

* Success
* Error

### RemoveResult

Is a result returned from a domain command the deletes a entity. It supports the following outcomes:

* Success
* NotFound ( The client can decied it this is the same a success )
* Error

### SearchResult

Is a result returned from a search query. It supports the following outcomes:

* Ok
* BadRequest ( Invalid search parameters )


### UpdatResult

Is an result returned from a domain command that modifies state. It supports the following outcomes:

* Success
* NotFound
* Error
 
 ## Version 

 ### Next Version

 * AddResult - Then that accepts an action has been added.

 ### Vr  0.1.0
 
 * AddResult
 * RemoveResult
 * SearchResult
 * UpdateResult