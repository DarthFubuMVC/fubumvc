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
        selectedBehaviors: ko.observableArray([]),
        addBehavior: function (name, id) {
            var self = this;
            var behavior = {
                Name: name,
                Id: id
            };
            behavior.isActive = ko.dependentObservable(function () {
                return self.currentBehavior() == behavior.Id;
            }, behavior);

            if (self.currentBehavior() == '') {
                self.currentBehavior(id);
            }
            this.selectedBehaviors.push(behavior);
        }
    };

    viewModel.currentBehavior.subscribe(function (value) {

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
    });

    viewModel.selectedBehaviors.subscribe(function () {
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
                item = '<li><a href="">' + behavior.Name + '</a></li>';
            }

            list.append(item);
        }
    });

    //ko.applyBindings(viewModel, document.getElementById('RequestBreadcrumb'));

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
});