var delay;
var trackUrl;

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


    function getAudioStreamUrl(bandAndSongName)
    {
  		var url = 'https://api.soundcloud.com/tracks.json?client_id=5ca9c93662aaa8d953a421ce53500bae&q=' + bandAndSongName;
		$.getJSON(url, function(tracks) {
		
			trackUrl = tracks[0].stream_url;
			 window.external.notify("onStreamUrlRetrieved");
		});
    }


    function getTrackUrl()
    {
		return trackUrl;
    }

	 function setAudioUrl(audioUrl) 
	 {
		var aud = document.getElementById("audio1");
		aud.setAttribute('src', audioUrl);
    }

 $(document).ready(function(){
    $("#audioSection").sticky({topSpacing:0});
    });