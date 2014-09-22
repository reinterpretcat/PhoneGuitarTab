var delay;
var trackUrl;
var trackTitle;
var isNetworkAvailable = false;
 
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
  		var url = 'https://api.soundcloud.com/tracks.json?client_id=5ca9c93662aaa8d953a421ce53500bae&q=' + artist + ' ' + song ;
		$.getJSON(url, function(tracks) {
        
		    $.each(tracks, function (i) {

		        if (tracks[i].streamable ) {

		            if (!(wordInString(tracks[i].title, 'cover')
		                    || wordInString(tracks[i].title, 'remix')
		                    || wordInString(tracks[i].title, 'dj')
                            || wordInString(tracks[i].title, 'drumstep')
                            || wordInString(tracks[i].title, 'dubstep')
                            || wordInString(tracks[i].title, 'edit')
                            || wordInString(tracks[i].title, 'rmx')
                            || wordInString(tracks[i].title, 'arrangement')
                            || wordInString(tracks[i].title, 'performs')
                            || wordInString(tracks[i].title, 'symphony')
		                    || tracks[i].title.toLowerCase().indexOf('cover') >= 0
                            || tracks[i].title.toLowerCase().indexOf('orchestra') >= 0
		            )) {
		                trackUrl = tracks[i].stream_url;
		                trackTitle = tracks[i].title;
		               
		                return false;
		            }
                  
                }    

			});

			isNetworkAvailable = true;
			setSticky();
			window.external.notify("onStreamUrlRetrieved");
		});
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


		