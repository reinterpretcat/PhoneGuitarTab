var delay;
var trackUrl;
var trackTitle;
var isNetworkAvailable = false;
 
function slide(interval) {
   clearInterval(delay);
    delay = setInterval( function () {
        window.scrollBy(0, 2)
    }, interval);
}

function stopSlide() {
    clearInterval(delay);
}

function pullTabContent(content) {
    document.getElementById('textContainer').innerHTML = content;
    }

function getAudioStreamUrl(bandAndSongName){
		setLabel("Loading " , bandAndSongName);
  		var url = 'https://api.soundcloud.com/tracks.json?client_id=5ca9c93662aaa8d953a421ce53500bae&q=' + bandAndSongName;
		$.getJSON(url, function(tracks) {
		
			trackUrl = tracks[0].stream_url;
			trackTitle = tracks[0].title;
			isNetworkAvailable = true;
			setSticky();
			window.external.notify("onStreamUrlRetrieved");
		});
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
		var subTitle = ": " + title.substring(0,50) + "..";
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


		