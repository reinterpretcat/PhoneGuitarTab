var context;var staveHelper;var paginator;var tab;
var scales = [0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2, 1.3];var scale = 0.6;
var trackCount = 0;var currentTrackIndex = 0;var tracks = [];var views;
var isNightMode = false;
$(document).ready(function () {
    initEnvironment();    window.external.notify("onReady");});function initEnvironment() {
    clearTab();	var height = $(document).height();
	var width = $(document).width();	context = new MusicTab.Stave.Context({						height: height,						width: width,						scale: scale,						backgroundColor: isNightMode ? "black" : "#eed",						fontColor: isNightMode ? "white" : "black",						placeHolderId: "body",						tabDivClass:"vex-tabdiv"	});	staveHelper = new MusicTab.Stave.Helper(context);	paginator = new MusicTab.Stave.Paginator({		context: context,		staveHelper: staveHelper	});}

function toggleLightMode(isNight) {
    isNightMode = isNight == "True";
    initEnvironment();
    showTab();
}

function readBase64(base64File) {
    (new MusicTab.Utils.FileReader()).read(base64File, function (data) {
        MusicTab.Tablatures.TabFactory.create({
                data: data,
                helper: staveHelper
            },
            function(tablature) {
                currentTrackIndex = 0;
                tab = tablature;
                processTab();
            });
    });}function readJson(jsonFile) {
    MusicTab.Tablatures.TabFactory.create({
            data: jsonFile,
            helper: staveHelper,
            type: "ug_json"
        },
        function (tablature) {
            currentTrackIndex = 0;
            tab = tablature;
            processTab();
        });
}

function nextTrack() {
    currentTrackIndex++;
    if (currentTrackIndex == trackCount)
        currentTrackIndex = 0;

    showTab();
}function prevTrack() {
    currentTrackIndex--;
    if (currentTrackIndex < 0)
        currentTrackIndex = trackCount - 1;

    showTab();
}//show the instrument with the specified indexfunction changeInstrument(trackIndex) {
    try {
        currentTrackIndex = parseInt(trackIndex);
        showTab();
    } catch(err) {
        alert(err.message)
    }
}//get instrument namefunction getInstrumentName(i) {
   return tracks[i].name;
}//get the lenght of the track arrayfunction getTrackCount() {
    return trackCount.toString();
}function scaleChange() {
    var i = 0;
    for(;i< scales.length;) {
        if(scales[i++] == scale) break;
    }
    
    if (i == scales.length) i = 0;
    scale = scales[i];
    
    showTab();
}

function clearTab() {

    if (views) {
        for (var j = 0; j < views.length; j++) {
            var view = views[j];
            $("#" + view.id).waypoint('destroy');
            view.destroy();
        }
    }

    var elems = $('#body div');
    for (var i = 0, elem; (elem = elems[i]) != null; i++) {
        jQuery.event.remove(elem);
        jQuery.removeData(elem);
        purge(elem);
    }
}function purge(d) {
    var a = d.childNodes;
    if (a) {
        var remove = false;
        while (!remove) {
            var l = a.length;
            for (i = 0; i < l; i += 1) {
                var child = a[i];
                if (child.childNodes.length == 0) {
                    jQuery.event.remove(child);
                    d.removeChild(child);
                    remove = true;
                    break;
                }
                else {
                    jQuery.purge(child);
                }
            }
            if (remove) {
                remove = false;
            } else {
                break;
            }
        }
    }
}function processTab() {	// create tracks	for (var i = 0; i < tab.tracks.length; i++) {		tracks.push(new MusicTab.Tablatures.Track({			name: tab.tracks[i].name,			instrument: tab.tracks[i].instrument,			selected: tab.tracks[i].selected,			index: i		}));			}
    trackCount = tracks.length;}

function showTab() {
    initEnvironment();
    var actualWidth = staveHelper.getActualWidth();
    var linePerPage = staveHelper.getLinePerPage();

    var measures = tab.tracks[currentTrackIndex].measures;
    var chunks = paginator.split(measures, actualWidth);

    var pages = paginator.doPaging(chunks, linePerPage);
    views = paginator.createViews(pages, tab.tracks[currentTrackIndex]);

    showViews();
}function showViews() {
    for (var i = 0; i < views.length; i++) {
        var view = views[i];
        if (i == 0) {
            view.show();
        } else {
            $("#" + view.id).waypoint(function () {
                // get index from id
                var index = parseInt(this.id.substring("vex-page".length, this.id.length)) - 1;
                if (!views[index].isShown) {
                    views[index].show();
                }
            },
            {
                offset: '50%'
            });
        }
    }
}