$(document).ready(function () {
    var arrow = $('#chain-arrow').html();
    $('.chain-visualizer > li:not(:last)').after('<li class="arrow">' + arrow + '</li>');

    $('.console > .message > ul').each(function () {
        $(this).find('li.binding-detail:last').each(function () {
            $(this).addClass('last');
        });
    });

    var viewModel = {
        currentBehavior: ko.observable(''),
        previousBehavior: '',
        selectedBehaviors: ko.observableArray([]),
        findBehavior: function (id) {
            var values = viewModel.selectedBehaviors();
            for (var i = 0; i < values.length; i++) {
                var x = values[i];
                if (x.Id == id) {
                    return x;
                }
            }

            return null;
        },
        addBehavior: function (name, id) {
            var self = this;
            var index = 0;

            var values = viewModel.selectedBehaviors();
            for (var i = 0; i < values.length; i++) {
                var x = values[i];
                if (x.index > index) {
                    index = x.index;
                }

                ++index;
            }

            var behavior = {
                Name: name,
                Id: id,
                index: index
            };

            behavior.isActive = function () {
                return self.currentBehavior() == behavior.Id;
            };

            if (self.currentBehavior() == '') {
                self.currentBehavior(id);
            }

            this.selectedBehaviors.push(behavior);
        },
        initialized: false
    };

    var batch = false;
    var resetBreadcrumb = function () {
        var list = $('#RequestBreadcrumb');
        list.html('');

        var value = viewModel.selectedBehaviors();
        for (var i = 0; i < value.length; i++) {
            var behavior = value[i];
            var item = '';
            if (behavior.isActive()) {
                item = '<li class="active">' + behavior.Name + '</li>';
            }
            else {
                item = '<li><a href="javascript:void(0);" class="{id: \'' + behavior.Id + '\'}">' + behavior.Name + '</a></li>';
            }

            list.append(item);

            if (i != (value.length - 1)) {
                list.append('<li> <span class="divider">/</span> </li>');
            }
        }

        $('#RequestBreadcrumb > li > a').click(function () {
            var id = $(this).metadata().id;
            var selectedBehavior = viewModel.findBehavior(id);
            batch = true;
            viewModel.selectedBehaviors.remove(function (x) {
                return x.index > selectedBehavior.index;
            });
            batch = false;

            viewModel.currentBehavior(id);
        });
    };

    viewModel.currentBehavior.subscribe(function (value) {
        if (viewModel.previousBehavior == value && viewModel.initialized) {
            return;
        }

        var chainNode = $('#Node-' + value);
        if (chainNode.size() != 0) {
            // update the chain visualizer
            $('.chain-node')
                .removeClass('primary')
                .removeClass('large');

            chainNode.addClass('primary');
            $('.chain-node').not('.primary').addClass('large');
        }

        // update the behavior visualizer
        $('.behavior').hide();
        $('#' + value).show('slow');

        if (!viewModel.initialized) {
            viewModel.initialized = true;
        }

        viewModel.previousBehavior = value;
        resetBreadcrumb();
    });

    $('.chain-node').click(function () {
        var self = $(this);
        if (self.hasClass('primary')) {
            return;
        }

        var id = self.attr('id').replace('Node-', '');
        if (!viewModel.findBehavior(id)) {
            viewModel.addBehavior($('#' + id + ' > h3').html(), id);
            viewModel.currentBehavior(id);
        }
        else {
            $('#RequestBreadcrumb > li > a').each(function () {
                var behaviorId = $(this).metadata().id;
                if (behaviorId == id) {
                    $(this).click();
                }
            });
        }

    });

    viewModel.selectedBehaviors.subscribe(function () {
        if (batch) {
            return;
        }
        resetBreadcrumb();
    });

    $('.behavior:first').each(function () {
        var self = $(this);
        viewModel.addBehavior(self.find('h3:first').html(), self.attr('id'));
    });

    $('.inner-behavior > a.btn').click(function () {
        var self = $(this);
        var id = self.metadata().id;
        viewModel.addBehavior(self.html(), id);
        viewModel.currentBehavior(id);
    });

    $('.exception-trigger').click(function () {
        $('.exception:first').each(function () {
            var self = $(this);
            self.parents('.behavior').each(function () {
                var id = $(this).attr('id');
                var newBehavior = viewModel.currentBehavior() != id;
                var delay = newBehavior ? 1000 : 0;

                viewModel.currentBehavior(id);
                setTimeout(function () {
                    $('html, body').animate({
                        scrollTop: self.offset().top
                    }, 1000);
                }, delay);
            });
        });
    });
});