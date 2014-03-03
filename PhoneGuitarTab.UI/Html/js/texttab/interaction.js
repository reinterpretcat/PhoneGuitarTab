var delay;
var trackUrl;
var trackTitle;
var isNetworkAvailable = false;

$(document).ready(function () {
    window.external.notify("onReady");
	
});
 

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
		setLabel("Loading stream for " , bandAndSongName);
  		var url = 'https://api.soundcloud.com/tracks.json?client_id=5ca9c93662aaa8d953a421ce53500bae&q=' + bandAndSongName;
		$.getJSON(url, function(tracks) {
		
			trackUrl = tracks[0].stream_url;
			trackTitle = tracks[0].title;
			isNetworkAvailable = true;
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
		title = "[ " + title + " ]";
		document.getElementById("info").innerHTML = text + title;
	}

function stopAudioPlayer(){
	document.getElementById("audio1").pause();
	}


 $(document).ready(function(){
	
	var aud = document.getElementById("audio1");
	
			aud.addEventListener('play', function(){
				if(isNetworkAvailable)
				{
					$("#audio1").sticky({topSpacing:0});
					setLabel("Playing " , trackTitle );
				}
				else
				{
					setLabel("Please check your network connection.." , "" );
				}
			});

			aud.addEventListener('pause', function(){
			setLabel("Paused " ,trackTitle );
			});

   });


		