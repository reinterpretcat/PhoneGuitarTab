MusicTab.namespace('MusicTab.Stave.Paginator');

MusicTab.Stave.Paginator = klass(null, {
    __construct: function (params) {
        this.init(params);
    },

    init: function (params) {
        this.context = params.context;
        this.staveHelper = params.staveHelper;
    },
    
    // insert tab pages
    insertPages: function(pages, track) {
        var root = document.getElementById(this.context.placeHolderId);
        var pageNumber = 1;
        var count = pages.length;
        var context = this.context;
		var views = [];
        pages.forEach(function(page) {
            var id = "vex-page" + pageNumber;
            var div = document.createElement("div");
            div.setAttribute("class", context.tabDivClass);
            div.setAttribute("id", id);
            div.setAttribute("label", pageNumber++ + " of " + count);
            root.appendChild(div);

            try {
			var view = new MusicTab.Stave.View({
                    selector: $("#" + id),
                    page: page,
                    track: track,
                    context: context
                });
				views.push(view);
                view.show();
            } catch(err) {
                console.log(err.message);
            }
        });
		return views;
    },

    doPaging: function(chunks, linesPerPage) {
        var pages = [];
        var page = [];
        var length = chunks.length;
        for (var i = 0; i < length; i++) {
            if (i % linesPerPage == 0 && i != 0) {
                pages.push(page);
                page = [];
            }
            page.push(chunks[i]);

            // latest iteration
            if (i == (length - 1)) {
                pages.push(page);
            }
        }
        return pages;
    },

    split: function(measures, lineWidth) {
        var length = measures.length;
        var chunks = [];
        var chunk = [];
        var sum = 0;

        var measureTreshold = lineWidth / 2;
        var lineTreshold = lineWidth - 120;
        //var avgMeasurePerLine = lineTreshold / 210; 

        var check = function(sum1, index, l) {
            return index == (length - 1) || // for the latest measure
                measures[index + 1].width > measureTreshold || // for really large measures
                (sum1 > measureTreshold && measures[index + 1].width > 200 ); // for medium measures when there is small space available
        };

        var time = measures[0].time;

        for (var i = 0; i < length; i++) {
            var inserted = false;

            // new time
            if (measures[i].time != time) {
                time = measures[i].time;
                measures[i].width += 20;
            }

            // first on line
            if (chunks.length == 0) {
                measures[i].width += 50;
            }

            var current = measures[i].width;
            sum += current;
            if (sum >= lineTreshold || check(sum, i, chunk.length)) {
                if (check(sum, i, chunk.length)) {
                    chunk.push(measures[i]);
                    inserted = true;
                    sum = 0;
                } else {
                    sum = current;
                }
                // TODO
                if (chunk.length > 0) {
                    chunks.push(this.staveHelper.adjustChunk(chunk));
                }
                chunk = [];
            }

            if (!inserted) {
                chunk.push(measures[i]);
            }
        }
        return chunks;
    }
});