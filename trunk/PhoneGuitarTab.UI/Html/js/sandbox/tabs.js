var context;var staveHelper;var paginator;var tab;
var scales = [0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2, 1.3];var scale = 0.3;
var trackCount = 0;var currentTrackIndex = 0;var views;$(document).ready(function () {    initEnvironment();    window.external.notify("onReady");});function initEnvironment() {
    clearTab();	var height = $(document).height();
    var width = $(document).width();	context = new MusicTab.Stave.Context({						height: height,						width: width,						scale: scale,						placeHolderId: "body",						tabDivClass:"vex-tabdiv"					});	staveHelper = new MusicTab.Stave.Helper(context);	paginator = new MusicTab.Stave.Paginator({		context: context,		staveHelper: staveHelper	});}function readBase64(base64File) {   
    (new MusicTab.Utils.FileReader()).read(base64File, function (data) {
        MusicTab.Tablatures.TabFactory.create({
                data: data,
                helper: staveHelper
            },
            function (tablature) {
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
}function scaleChange() {
    var i = 0;
    for(;i< scales.length;) {
        if(scales[i++] == scale) break;
    }
    
    if (i == scales.length) i = 0;
    scale = scales[i];
    
    showTab();
}

function clearTab(){	$('#body div').html('');}function processTab() {	// create tracks	var tracks = [];	//$('#tracks').html('');	for (var i = 0; i < tab.tracks.length; i++) {		tracks.push(new MusicTab.Tablatures.Track({			name: tab.tracks[i].name,			instrument: tab.tracks[i].instrument,			selected: tab.tracks[i].selected,			index: i		}));				/*$('#tracks')			 .append($("<option></option>")			 .attr("value", i)			 .text(tab.tracks[i].name)); */	}
    trackCount = tracks.length;	//show tab	showTab();}function showTab(){	initEnvironment();	var actualWidth = staveHelper.getActualWidth();	var linePerPage = staveHelper.getLinePerPage();		var measures = tab.tracks[currentTrackIndex].measures;	var chunks = paginator.split(measures, actualWidth);	var pages = paginator.doPaging(chunks, linePerPage);
    //insertTitle();	views = paginator.insertPages(pages, tab.tracks[currentTrackIndex]);}/*function insertTitle() {

    var tracks = [{
        artist: tab.header.artist,
        album: tab.header.album,
        title: tab.header.title,
        music: tab.header.music,
        words: tab.header.words,
        notice: tab.header.notice,
        instrument: tab.tracks[currentTrackIndex].instrument
    }];
    
    $("#trackTmpl").tmpl(tracks).appendTo("#body");
}*/