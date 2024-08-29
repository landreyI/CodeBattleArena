$(document).ready(function () {

    var InputDataList = [];
    function updateInputDataList() {
        $('.wrapperId-div').each(function () {
            const inputData = $(this).find('input[name="inputDataList[]"]');
            const idInputData = parseInt(inputData.attr('data-idInputData'));
            const data = inputData.val();

            InputDataList.push({ id: idInputData, data: data });
        });
    }

    // Обновляем список при загрузке страницы
    updateInputDataList();

    $(document).on('click', '.btn-add-inputData', function (event) {
        event.preventDefault();

        var button = $(this);
        var inputDataId = parseInt(button.data('inputdata-id'));
        var buttonText = button.text();
        var isSelected = button.data('selected');

        if (isSelected) {
            button.data('selected', false).removeClass('btn-success').addClass('btn-light');
            // Удаление из массива по ID
            InputDataList = InputDataList.filter(item => item.id !== inputDataId);
        } else {
            button.data('selected', true).removeClass('btn-light').addClass('btn-success');
            // Добавление ID и текста в массив
            InputDataList.push({ id: inputDataId, data: buttonText });
        }

    });

    $('#inputDataForm').submit(function (event) {
        event.preventDefault();
        var form = $(this);

        var addTaskForm = $("#addTaskForm");
        var inputDataSelectDiv = addTaskForm.find("#InputDataSelect");

        // Пройти по каждому скрытому полю с name="idInputDataList"
        var existingInputs = addTaskForm.find('input[type="text"][name="idInputDataList[]"]');

        // Пробегаемся по списку InputDataList
        InputDataList.forEach(function (item) {
            // Проверка, существует ли элемент с данным id
            var exists = existingInputs.filter(function () {
                return $(this).attr('data-id') == item.id;
            }).length > 0;

            // Если элемента не существует, добавляем его
            if (!exists) {
                // Создание обёрточного div
                const wrapperIdDiv = $('<div>').addClass('wrapperId-div');

                // Создание основного содержимого внутри обёртки
                const contentDiv = $('<div>').addClass('d-flex').append(
                    $('<input>').attr({
                        type: 'text',
                        class: 'form-control me-2 my-3',
                        value: item.data,
                        readonly: true
                    }),
                    $('<input>').attr({
                        type: 'text',
                        class: 'form-control me-2 my-3',
                        name: 'idInputDataList[]',
                        'data-id': item.id
                    }),
                    $('<button>')
                        .addClass('btn btn-danger my-3 delete-button')
                        .html('🗑️')
                );

                wrapperIdDiv.append(contentDiv);

                // Добавление обёртки и скрытого поля в основной контейнер
                inputDataSelectDiv.append(wrapperIdDiv);
            }
        });

        // Закрываем модальное окно после отправки формы
        form.closest('.modal').modal('hide');
    });

    document.getElementById('addInputDataBtn').addEventListener('click', function (event) {
        event.preventDefault();
        var inputDataSelectDiv = $('#InputDataSelect');

        const wrapperCreateDiv = $('<div>').addClass('wrapperCreate-div');

        // Создание основного содержимого внутри обёртки
        const contentDiv = $('<div>').addClass('d-flex').append(
            $('<input>').attr({
                type: 'text',
                class: 'form-control me-2 my-3',
                name: 'inputDataList[]'
            }),
            $('<input>').attr({
                type: 'text',
                class: 'form-control  me-2 my-3',
                name: 'answerList[]',
            }),
            $('<button>')
                .addClass('btn btn-danger my-3 delete-buttonCreate')
                .html('🗑️')
        );

        wrapperCreateDiv.append(contentDiv);

        // Добавление обёртки и скрытого поля в основной контейнер
        inputDataSelectDiv.append(wrapperCreateDiv);

        // Добавление обработчика события на кнопку удаления
        wrapperCreateDiv.find('.delete-buttonCreate').on('click', function () {
            // Удаление wrapperCreateDiv из DOM
            wrapperCreateDiv.remove();
        });
    });

    document.getElementById('deleteInputDataBtn').addEventListener('click', function (event) {
        event.preventDefault();
        var inputDataSelectDiv = $('#InputDataSelect');

        inputDataSelectDiv.empty();
        InputDataList = [];
    });

    document.getElementById('showModalInputData').addEventListener('click', function (event) {
        event.preventDefault();

        var addTaskForm = $("#addTaskForm");
        var listInputData = addTaskForm.find('input[type="hidden"][data-idinputdata]').each(function () {
            // Считываем данные атрибутов
            var inputDataId = $(this).data('idinputdata');
            var buttonText = $(this).data('textinputdata');

            // Добавляем в массив
            InputDataList.push({ id: inputDataId, data: buttonText });
        });

        var inputDataForm = $("#inputDataForm");
        var listInputData = inputDataForm.find("#listInputData");

        listInputData.find('button').each(function () {
            var button = $(this);
            var buttonId = parseInt(button.data('inputdata-id'));
            button.show();
            button.removeClass('btn-success').addClass('btn-light');
            // Проверяем, есть ли buttonId в InputDataList
            var found = InputDataList.some(function (item) {
                return item.id === buttonId;
            });

            // Если найдено совпадение, скрываем кнопку
            if (found) {
                button.hide();
            }
        });

    });



    $('#addTaskForm').submit(function (event) {
        event.preventDefault();

        let inputDataAddId = [];
        let inputDataCreate = [];

        let isValid = true;

        $('#InputDataSelect').find('.wrapperId-div').each(function () {
            const inputData = $(this).find('input[type="text"][name="idInputDataList[]"]');

            const idInputData = parseInt(inputData.attr('data-id'));
            const answer = inputData.val();

            // Проверяем, пустое ли поле ответа
            if (answer.trim() === '') {
                isValid = false;
                $(this).find('.error-message').remove();
                $(this).append('<div class="error-message text-danger">The response field cannot be empty!</div>');
            } else {
                // Удаляем сообщение об ошибке, если поле заполнено
                $(this).find('.error-message').remove();
            }

            inputDataAddId.push({ InputData: idInputData, Answer: answer });
        });

        $('#InputDataSelect').find('.wrapperCreate-div').each(function () {
            const inputData = $(this).find('input[type="text"][name="inputDataList[]"]').val();
            const answer = $(this).find('input[type="text"][name="answerList[]"]').val();

            if (answer.trim() === '' || inputData.trim() === '') {
                isValid = false;
                $(this).find('.error-message').remove();
                $(this).append('<div class="error-message text-danger">The response field cannot be empty!</div>');
            } else {
                $(this).find('.error-message').remove();
            }

            inputDataCreate.push({ InputData: inputData, Answer: answer });
        });

        if (isValid) {

            $('#addTaskForm').find('input[name="inputDataAddIdJson"]').val(JSON.stringify(inputDataAddId));
            $('#addTaskForm').find('input[name="inputDataCreateJson"]').val(JSON.stringify(inputDataCreate));

            $('#addTaskForm').unbind('submit').submit();
            
        } else {
            console.log('The form is invalid. Correct the errors.');
        }
    });

    $(document).on('click', '.delete-button', function () {
        var button = $(this);
        var wrapperIdDiv = button.closest('.wrapperId-div');
        var answerInput = wrapperIdDiv.find('input[name="idInputDataList[]"]');

        var inputDataId = parseInt(answerInput.attr('data-id'));

        InputDataList = InputDataList.filter(item => item.id !== inputDataId);

        wrapperIdDiv.remove();
    });
});