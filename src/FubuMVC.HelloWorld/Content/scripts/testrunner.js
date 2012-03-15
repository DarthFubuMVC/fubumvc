/*
 * QUnit - jQuery unit testrunner
 * 
 * http://docs.jquery.com/QUnit
 *
 * Copyright (c) 2008 John Resig, Jörn Zaefferer
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * $Id: testrunner.js 5716 2008-06-07 02:46:52Z rdworth $
 */
/// <reference path="../shared/jquery-1.4.2.js" />

// This snippet allows you to drag a test.htm file from Windows Explorer into the browser
// and the test will redirect you from the file:/// url to the proper http:// URL
if( window.location.href.length > 7 && window.location.href.substring(0, 7) == "file://" ){
    var fileStripped = window.location.href.replace(/^file:\/\/\/[A-Z]\:\/[A-Za-z0-9\/]+?\/source\/DovetailCRM\.Web\/Content/i, "http://localhost/DovetailCRM/Content");
    window.location.href = fileStripped;
}

var _config = {
	fixture: null,
	Test: [],
	stats: {
		all: 0,
		good: 0,
		bad: 0
	},
	queue: [],
	blocking: true,
	timeout: null,
	expected: null,
	currentModule: null,
	startTime : new Date().getTime(),
	beforeEach : null,
	afterEach : null,
	asyncTimeout: 2 // seconds for async timeout
	
};

_config.filters = location.search.length > 1 && //restrict modules/tests by get parameters
		$.map( location.search.slice(1).split('&'), decodeURIComponent );

var isLocal = !!(window.location.protocol == 'file:');

$(function() {
	$('#userAgent').html(navigator.userAgent);
	runTest();	
});

function qUnitTesting( testsCallback ){
    $(document).ready(function(){
        testsCallback(_config);
        allTestsDone();        
    });
}

function synchronize(callback) {
	_config.queue[_config.queue.length] = callback;
	if(!_config.blocking) {
		process();
	}
}

function process() {
	while(_config.queue.length && !_config.blocking) {
		var call = _config.queue[0];
		_config.queue = _config.queue.slice(1);
		call();
	}
}

function stop(allowFailure) {
	_config.blocking = true;
	var handler = allowFailure ? start : function() {
		ok( false, "Test timed out" );
		start();
	};
	// Disabled, caused too many random errors
	//_config.timeout = setTimeout(handler, _config.asyncTimeout * 1000);
}
function start() {
	// A slight delay, to avoid any current callbacks
	setTimeout(function(){
		if(_config.timeout)
			clearTimeout(_config.timeout);
		_config.blocking = false;
		process();
	}, 13);
}

function validTest( name ) {
	var filters = _config.filters;
	if( !filters )
		return true;

	var i = filters.length,
		run = false;
	while( i-- ){
		var filter = filters[i],
			not = filter.charAt(0) == '!';
		if( not ) 
			filter = filter.slice(1);
		if( name.indexOf(filter) != -1 )
			return !not;
		if( not )
			run = true;
	}
	return run;
}

function runTest() {
	_config.blocking = false;	
	_config.fixture = document.getElementById('main').innerHTML;
	_config.ajaxSettings = $.ajaxSettings;
}

function allTestsDone(){
    synchronize(function(){
        updateStats();
        $("<div>").attr("id", "TESTRESULTS").html(""+_config.stats.bad).hide().appendTo("body");        
    });
}

function updateStats(){
	synchronize(function() {
		_config.endTime = new Date().getTime();
		$("#results").html(['<p class="result">Tests completed in ',
			(_config.endTime - _config.startTime), ' milliseconds.<br/>',
			_config.stats.good, ' tests of ', _config.stats.all, ' passed.<br/>',
			_config.stats.bad, ' tests of ', _config.stats.all, ' failed.</p>']
			.join(''));
		$("#banner").addClass(_config.stats.bad ? "fail" : "pass");
	});
}

function test(name, callback, nowait) {
	if(_config.currentModule)
		name = _config.currentModule + " module: " + name;
		
    var beforeEach = _config.beforeEach || function(){}
    var afterEach = _config.afterEach || function(){}
		
	if ( !validTest(name) )
		return;
		
	synchronize(function() {
		_config.Test = [];
		_config.currentTestName = name;
		try {		    
			beforeEach(name);
			callback();
			afterEach(name);
		} catch(e) {
			if( typeof console != "undefined" && console.error && console.warn ) {
				console.error("Test " + name + " died, exception and test follows");
				console.error(e);
				console.warn(callback.toString());
			}
			_config.Test.push( [ false, "Died on test #" + (_config.Test.length+1) + ": " + e.message ] );
		}
		_config.currentTestName = null;
	});
	synchronize(function() {
		try {
			reset();
		} catch(e) {
			if( typeof console != "undefined" && console.error && console.warn ) {
				console.error("reset() failed, following Test " + name + ", exception and reset fn follows");
				console.error(e);
				console.warn(reset.toString());
			}
		}
		
		// don't output pause tests
		if(nowait) return;
		
		if(_config.expected && _config.expected != _config.Test.length) {
			_config.Test.push( [ false, "Expected " + _config.expected + " assertions, but " + _config.Test.length + " were run" ] );
		}
		_config.expected = null;
		
		var good = 0, bad = 0;
		var ol = document.createElement("ol");
		ol.style.display = "none";
		var li = "", state = "pass";
		for ( var i = 0; i < _config.Test.length; i++ ) {
			var li = document.createElement("li");
			li.className = _config.Test[i][0] ? "pass" : "fail";
			li.innerHTML = _config.Test[i][1];
			ol.appendChild( li );
			
			_config.stats.all++;
			if ( !_config.Test[i][0] ) {
				state = "fail";
				bad++;
				_config.stats.bad++;
			} else {
			    good++;
			    _config.stats.good++;
			}
		}
	
		var li = document.createElement("li");
		li.className = state;
	
		var b = document.createElement("strong");
		b.innerHTML = name + " <b style='color:black;'>(<b class='fail'>" + bad + "</b>, <b class='pass'>" + good + "</b>, " + _config.Test.length + ")</b>";
		b.onclick = function(){
			var n = this.nextSibling;
			if ( jQuery.css( n, "display" ) == "none" )
				n.style.display = "block";
			else
				n.style.display = "none";
		};
		$(b).dblclick(function(event) {
			var target = jQuery(event.target).filter("strong").clone();
			if ( target.length ) {
				target.children().remove();
				location.href = location.href.match(/^(.+?)(\?.*)?$/)[1] + "?" + encodeURIComponent($.trim(target.text()));
			}
		});
		li.appendChild( b );
		li.appendChild( ol );
	
		document.getElementById("tests").appendChild( li );		
	});	
}

// call on start of module test to prepend name to all tests
function module(moduleName) {
	_config.currentModule = moduleName;
}

/**
 * Specify the number of expected assertions to gurantee that failed test (no assertions are run at all) don't slip through.
 */
function expect(asserts) {
	_config.expected = asserts;
}

/**
 * Resets the test setup. Useful for tests that modify the DOM.
 */
function reset() {
	$("#main").html( _config.fixture );
	$.event.global = {};
	$.ajaxSettings = $.extend({}, _config.ajaxSettings);
}

/**
 * Asserts true.
 * @example ok( $("a").size() > 5, "There must be at least 5 anchors" );
 */
function ok(a, msg) {
	_config.Test.push( [ !!a, msg ] );
}

/**
 * Asserts that two arrays are the same
 */
function isSet(a, b, msg) {
	var ret = true;
	if ( a && b && a.length != undefined && a.length == b.length ) {
		for ( var i = 0; i < a.length; i++ )
			if ( a[i] != b[i] )
				ret = false;
	} else
		ret = false;
	if ( !ret )
		_config.Test.push( [ ret, msg + " expected: " + serialArray(b) + " result: " + serialArray(a) ] );
	else 
		_config.Test.push( [ ret, msg ] );
}

/**
 * Asserts that two objects are equivalent
 */
function isObj(a, b, msg) {
	var ret = true;
	
	if ( a && b ) {
		for ( var i in a )
			if ( a[i] != b[i] )
				ret = false;

		for ( i in b )
			if ( a[i] != b[i] )
				ret = false;
	} else
		ret = false;

    _config.Test.push( [ ret, msg ] );
}

function serialArray( a ) {
	var r = [];
	
	if ( a && a.length )
        for ( var i = 0; i < a.length; i++ ) {
            var str = a[i].nodeName;
            if ( str ) {
                str = str.toLowerCase();
                if ( a[i].id )
                    str += "#" + a[i].id;
            } else
                str = a[i];
            r.push( str );
        }

	return "[ " + r.join(", ") + " ]";
}

function flatMap(a, block) {
	var result = [];
	$.each(a, function() {
		var x = block.apply(this, arguments);
		if (x !== false)
			result.push(x);
	})
	return result;
}

function serialObject( a ) {
	return "{ " + flatMap(a, function(key, value) {
		return key + ": " + value;
	}).join(", ") + " }";
}

function compare(a, b, msg) {
	var ret = true;
	if ( a && b && a.length != undefined && a.length == b.length ) {
		for ( var i = 0; i < a.length; i++ )
			for(var key in a[i]) {
				if (a[i][key].constructor == Array) {
					for (var arrayKey in a[i][key]) {
						if (a[i][key][arrayKey] != b[i][key][arrayKey]) {
							ret = false;
						}
					}
				} else if (a[i][key] != b[i][key]) {
					ret = false
				}
			}
	} else
		ret = false;
	ok( ret, msg + " expected: " + serialArray(b) + " result: " + serialArray(a) );
}

function compare2(a, b, msg) {
	var ret = true;
	if ( a && b ) {
		for(var key in a) {
			if (a[key].constructor == Array) {
				for (var arrayKey in a[key]) {
					if (a[key][arrayKey] != b[key][arrayKey]) {
						ret = false;
					}
				}
			} else if (a[key] != b[key]) {
				ret = false
			}
		}
		for(key in b) {
			if (b[key].constructor == Array) {
				for (var arrayKey in b[key]) {
					if (a[key][arrayKey] != b[key][arrayKey]) {
						ret = false;
					}
				}
			} else if (a[key] != b[key]) {
				ret = false
			}
		}
	} else
		ret = false;
	ok( ret, msg + " expected: " + serialObject(b) + " result: " + serialObject(a) );
}

/**
 * Returns an array of elements with the given IDs, eg.
 * @example q("main", "foo", "bar")
 * @result [<div id="main">, <span id="foo">, <input id="bar">]
 */
function q() {
	var r = [];
	for ( var i = 0; i < arguments.length; i++ )
		r.push( document.getElementById( arguments[i] ) );
	return r;
}

/**
 * Asserts that a select matches the given IDs
 * @example t("Check for something", "//[a]", ["foo", "baar"]);
 * @result returns true if "//[a]" return two elements with the IDs 'foo' and 'baar'
 */
function t(a,b,c) {
	var f = jQuery(b);
	var s = "";
	for ( var i = 0; i < f.length; i++ )
		s += (s && ",") + '"' + f[i].id + '"';
	isSet(f, q.apply(q,c), a + " (" + b + ")");
}

/**
 * Add random number to url to stop IE from caching
 *
 * @example url("data/test.html")
 * @result "data/test.html?10538358428943"
 *
 * @example url("data/test.php?foo=bar")
 * @result "data/test.php?foo=bar&10538358345554"
 */
function url(value) {
	return value + (/\?/.test(value) ? "&" : "?") + new Date().getTime() + "" + parseInt(Math.random()*100000);
}

/**
 * Checks that the first two arguments are equal, with an optional message.
 * Prints out both expected and actual values on failure.
 *
 * Prefered to ok( expected == actual, message )
 *
 * @example equals( "Expected 2 characters.", v.formatMessage("Expected {0} characters.", 2) );
 *
 * @param Object actual
 * @param Object expected
 * @param String message (optional)
 */
function equals(actual, expected, message) {
	var result = expected == actual;
	message = message || (result ? "okay" : "failed");
	_config.Test.push( [ result, result ? message + ": " + expected : message + " expected: " + expected + " actual: " + actual ] );
}

/**
 * Trigger an event on an element.
 *
 * @example triggerEvent( document.body, "click" );
 *
 * @param DOMElement elem
 * @param String type
 */
function triggerEvent( elem, type, event ) {
	if ( jQuery.browser.mozilla || jQuery.browser.opera ) {
		event = document.createEvent("MouseEvents");
		event.initMouseEvent(type, true, true, elem.ownerDocument.defaultView,
			0, 0, 0, 0, 0, false, false, false, false, 0, null);
		elem.dispatchEvent( event );
	} else if ( jQuery.browser.msie ) {
		elem.fireEvent("on"+type);
	}
}
