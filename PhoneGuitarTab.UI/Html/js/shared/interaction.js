var delay;
var trackUrl;
var trackTitle;
var isNetworkAvailable = false;
var isAudioFound = false;

function slide(interval) {
   clearInterval(delay);
    delay = setInterval( function () {
        window.scrollBy(0, 2);
    }, interval);
}

function stopSlide() {
    clearInterval(delay);
}

function getAudioStreamUrl(artist, song){
		setLabel("Loading Audio" , artist + ' - ' + song);
        isAudioFound = false;
  		var url = 'https://api.soundcloud.com/tracks.json?client_id=5ca9c93662aaa8d953a421ce53500bae&q=' + artist + ' ' + song ;
		$.getJSON(url, function(tracks) {
        
            
            //Advanced filters to find the 'almost' exact match of the given song; if both ARTIST and SONG exist in title.
		    $.each(tracks, function (i) {

		        if (tracks[i].streamable
                    && tracks[i].title.toLowerCase().indexOf(artist.toLowerCase()) >= 0
                    && tracks[i].title.toLowerCase().indexOf(song.toLowerCase()) >= 0
                    ){
		            if (isTitleClean(tracks[i].title.toLowerCase())) {	
                        
		                assignAudioVariablesAndNotify( tracks[i].title, tracks[i].stream_url);
                        //break out from the loop
		                return false;
		            }
                  
                }    

			});

            //If a match couldn't be found from the above case, search with a lighter filter; if ARTIST name exists in title
		   if (!isAudioFound) {

		       $.each(tracks, function (i) {

		           if (tracks[i].streamable
                       && tracks[i].title.toLowerCase().indexOf(artist.toLowerCase()) >= 0) {
		               if (isTitleClean(tracks[i].title.toLowerCase())) {

		                   assignAudioVariablesAndNotify(tracks[i].title, tracks[i].stream_url);
		                   //break out from the loop
		                   return false;
		               }

		           }

		       });
		   }
		   
		    //If a match couldn't be found from the above case search, with lighter filter: if SONG name exists in title
		   if (!isAudioFound) {

		       $.each(tracks, function (i) {

		           if (tracks[i].streamable
                       && tracks[i].title.toLowerCase().indexOf(song.toLowerCase()) >= 0) {
		               if (isTitleClean(tracks[i].title.toLowerCase())) {

		                   assignAudioVariablesAndNotify(tracks[i].title, tracks[i].stream_url);
		                   //break out from the loop
		                   return false;
		               }

		           }

		       });
		   }

		});
    }

function assignAudioVariablesAndNotify(title, streamUrl) {
    trackUrl = streamUrl;
    trackTitle = title;
    isNetworkAvailable = true;
    isAudioFound = true;
    setSticky();
    window.external.notify("onStreamUrlRetrieved");
}

function isTitleClean(songTitle) {

    if (wordInString(songTitle, 'cover')
        || wordInString(songTitle, 'remix')
        || wordInString(songTitle, 'ft')
        || wordInString(songTitle, 'feat')
        || wordInString(songTitle, 'version')
        || wordInString(songTitle, 'remake')
        || wordInString(songTitle, 'studio')
        || wordInString(songTitle, 'dj')
        || wordInString(songTitle, 'drumstep')
        || wordInString(songTitle, 'dubstep')
        || wordInString(songTitle, 'edit')
        || wordInString(songTitle, 'rmx')
        || wordInString(songTitle, 'arrangement')
        || wordInString(songTitle, 'mix')
        || wordInString(songTitle, 'mixed')
        || wordInString(songTitle, 'performs')
        || songTitle.toLowerCase().indexOf('cover') >= 0
        || songTitle.toLowerCase().indexOf('intro') >= 0
        || songTitle.toLowerCase().indexOf('orchestra') >= 0)
        { return false;}      
        else
        { return true; }
    }


function wordInString(s, word) {
    return new RegExp('\\b' + word + '\\b', 'i').test(s.toLowerCase());
}



function getTrackUrl() {
		return trackUrl;
    }

function setAudioUrl(audioUrl) {
		var aud = document.getElementById("audio1");
		aud.setAttribute('src', audioUrl);
		setLabel("Play " , trackTitle );
    }

function setLabel(text, title) {
		var subTitle = ": " + title.substring(0,45) + "..";
		document.getElementById("info").innerHTML = text + subTitle;
	}

function stopAudioPlayer(){
if( $(document).ready )
	{
		document.getElementById("audio1").pause();
	}
}
function setSticky() {
				
$("#audioSection").sticky({topSpacing:-20});					
}


$(function() {

    var aud = document.getElementById("audio1");

    aud.addEventListener('playing', function() {
        if (isNetworkAvailable) {
            setLabel("Playing ", trackTitle);

        } else {
            setLabel("Please check your network connection..", "");
        }
    });

    aud.addEventListener('pause', function() {
        setLabel("Paused ", trackTitle);
    });

});


		