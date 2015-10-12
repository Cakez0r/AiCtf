var Player = function(gamefile, downloadProgressHandler, frameHandler) {
    var self = this;

    var timer = 0;

    var fps = 30;

    var currentFrame = 0;

    this.isLoaded = false;

    this.loadingProgress = 0;

    this.game = {};

    this.lastFrameTime = 0;

    this.currentState = {};

    this.playbackProgress = 0;

    this.play = function() {
        if (!self.isLoaded) {
            return;
        }

        self.stop();

        self.lastFrameTime = new Date().getTime();
        timer = setInterval(function() {
            self.step();
        }, 1000 / Math.min(fps, 30));
    }

    this.getPlaybackFps = function() {
        return fps;
    }

    this.setPlaybackFps = function(f) {
        fps = f;
    }

    this.stop = function() {
        if (timer) {
            clearInterval(timer);
        }
    }

    this.step = function() {
        if (!self.isLoaded) {
            return;
        }

        var t = new Date().getTime();
        var elapsed = t - self.lastFrameTime;
        var f = parseInt(currentFrame);

        currentFrame += fps * (elapsed / 1000);
        currentFrame = Math.max(0, Math.min(currentFrame, self.game.States.length - 1));

        var newFrame = parseInt(currentFrame);
        if (newFrame != f) {
            self.setFrame(newFrame);
        }

        self.lastFrameTime = t;
    }

    this.setFrame = function(f) {
        if (!self.isLoaded) {
            return;
        }

        f = Math.max(0, Math.min(f, self.game.States.length - 1));

        currentFrame = f;
        self.currentState = self.game.States[f];
        self.playbackProgress = f / self.game.States.length;

        if (frameHandler) {
            frameHandler(self.currentState);
        }
    }

    this.setPlaybackProgress = function(p) {
        if (!self.isLoaded) {
            return;
        }

        p = Math.max(0, Math.min(p, 1));
        self.setFrame(p * self.game.States.length);
    }

    $.ajax({
        type: 'GET',

        url: gamefile,

        xhr: function() {
            var xhr = new window.XMLHttpRequest();

            xhr.addEventListener("progress", function(evt){
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    self.loadingProgress = percentComplete;

                    if (progressHandler) {
                        progressHandler(percentComplete);
                    }
                }
            }, false);

            return xhr;
        },

        success: function(data) {
            self.game = data;
            self.setFrame(0);

            if (progressHandler){
                self.isLoaded = true;
                progressHandler(1);
            }
        },

        error: function() {
        }
    });
}
