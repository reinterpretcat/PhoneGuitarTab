MusicTab.namespace('MusicTab.Stave.Helper');

/* encapsulates migic constants */
MusicTab.Stave.Helper = klass(null, {
    __construct: function(params) {
        this.init(params);
    },
    
    init: function (params) {
        
        this.scale = params.scale || 1.0;
        this.width = (params.width || 400) / this.scale;
        this.height = (params.height || 200) / this.scale;


        this.oneLineHeight = 230;

        this.actualWidth = this.width;// - 40 * this.scale;
        this.actualHeight = this.height - 20 * this.scale;
        this.linePerPage = (this.actualHeight - this.actualHeight % this.oneLineHeight) / this.oneLineHeight;
    },
    
    getActualHeight: function () {
        return this.actualHeight;
    },
    
    getActualWidth: function () {
        return this.actualWidth - 40; //??
    },
    
    getLinePerPage: function(){
        return this.linePerPage;
    },
    
    getLastLineMeasureLength: function (offsetX) {
        return this.getActualWidth() - offsetX + 30;
    },
    
    getMeasureWidth: function (beats) {
        var count = beats.length;
        var result = 0;
        if (count > 25) {
            result = count > 1 ? (44 * count) + 10 * this.scale : 60;
        }
        else if (count > 15)
        {
            result = count > 1 ? ((130 * Math.log(count))) + 10 * this.scale : 60;
        } else {
            result = count > 1 ? ((90 * Math.log(count))) + 15 * this.scale : 60;
        }

        return Math.min(result, this.getActualWidth());
    },

   
    getLineGap: function () {
        return (this.height - (this.linePerPage * this.oneLineHeight)) / this.linePerPage;
    },
    
    adjustChunk: function (measures) {
        // adjust measure length;
        var sum = 0;
        var i;
        var largeMeasures = 0;
        var minNoteCount = 2;
        for (i = 0; i < measures.length; i++) {
            sum += measures[i].width;
            if (measures[i].beats.length > minNoteCount) {
                largeMeasures++;
            }
        }
        
        var delta = (this.getActualWidth() - sum) / (largeMeasures > 0 ? largeMeasures : measures.length);

        for (i = 0; i < measures.length; i++) {
            measures[i].adjustedWidth = measures[i].width;
            if ((largeMeasures == 0) || (largeMeasures > 0 && measures[i].beats.length > minNoteCount)) {
                measures[i].adjustedWidth += delta;
            }
        }
        return measures;
    }
});