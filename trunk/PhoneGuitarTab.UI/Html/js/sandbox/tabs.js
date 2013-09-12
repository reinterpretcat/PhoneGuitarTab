var context;var staveHelper;var paginator;var tab;var scale = 0.4;
var trackCount = 0;var currentTrackIndex = 0;var views;$(document).ready(function () {    initEnvironment();    window.external.notify("onReady");});function initEnvironment() {
    clearTab();	var height = $(document).height();	var width = $(document).width() - 40;	context = new MusicTab.Stave.Context({						height: height,						width: width,						scale: scale,						placeHolderId: "body",						tabDivClass:"vex-tabdiv"					});	staveHelper = new MusicTab.Stave.Helper(context);	paginator = new MusicTab.Stave.Paginator({		context: context,		staveHelper: staveHelper	});}function readBase64(base64File) {   
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
}  function clearTab(){	$('#body div').html('');}function processTab() {	// create tracks	var tracks = [];	//$('#tracks').html('');	for (var i = 0; i < tab.tracks.length; i++) {		tracks.push(new MusicTab.Tablatures.Track({			name: tab.tracks[i].name,			instrument: tab.tracks[i].instrument,			selected: tab.tracks[i].selected,			index: i		}));				/*$('#tracks')			 .append($("<option></option>")			 .attr("value", i)			 .text(tab.tracks[i].name)); */	}
    trackCount = tracks.length;	//show tab	showTab();}function showTab(){	initEnvironment();	var actualWidth = staveHelper.getActualWidth();	var linePerPage = staveHelper.getLinePerPage();		var measures = tab.tracks[currentTrackIndex].measures;	var chunks = paginator.split(measures, actualWidth);	var pages = paginator.doPaging(chunks, linePerPage);
    //insertTitle();	views = paginator.insertPages(pages, tab.tracks[currentTrackIndex]);}function insertTitle() {

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
}