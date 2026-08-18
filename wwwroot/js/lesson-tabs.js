document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll("[data-lesson-tab]");
    const panels = document.querySelectorAll("[data-lesson-panel]");

    if (!tabs.length || !panels.length) {
        return;
    }

    tabs.forEach(function (tab) {
        tab.addEventListener("click", function () {
            const target = tab.dataset.lessonTab;

            tabs.forEach(function (item) {
                item.classList.toggle("active", item === tab);
            });

            panels.forEach(function (panel) {
                panel.classList.toggle(
                    "active",
                    panel.dataset.lessonPanel === target
                );
            });
        });
    });
});


// Section Vocabulary

document.addEventListener("DOMContentLoaded", () => {

    const dataElement = document.getElementById("vocabulary-data");

    if (!dataElement) {
        return;
    }

    const vocabularies = JSON.parse(dataElement.textContent);

    if (!vocabularies.length) {
        return;
    }

    let currentIndex = 0;

    let selectedMeaning = null;
    let selectedListeningWord = null;


    const wordElement = document.getElementById("vocabulary-word");

    const pronunciationElement = document.getElementById("vocabulary-pronunciation");

    const meaningElement = document.getElementById("vocabulary-meaning");

    const exampleElement = document.getElementById("vocabulary-example");

    const guessWordElement = document.getElementById("guess-word");

    const meaningGrid = document.getElementById("meaning-answer-grid");

    const listeningGrid = document.getElementById("listening-answer-grid");

    const meaningResult = document.getElementById("meaning-result");

    const listeningResult = document.getElementById("listening-result");

    const positionElement = document.getElementById("vocabulary-position");

    const audioElement = document.getElementById("vocabulary-audio");


    function currentVocabulary() {
        return vocabularies[currentIndex];
    }

    function shuffle(array) {

        const result = [...array];

        for (let i = result.length - 1; i > 0; i--) {

            const j =
                Math.floor(Math.random() * (i + 1));

            [result[i], result[j]] =
                [result[j], result[i]];
        }

        return result;
    }


    function renderVocabulary() {

        const vocabulary = currentVocabulary();

        wordElement.textContent = vocabulary.word;

        pronunciationElement.textContent = vocabulary.pronunciation || "";

        meaningElement.textContent = vocabulary.meaning;

        exampleElement.textContent = vocabulary.example || "Chưa có ví dụ.";

        guessWordElement.textContent = vocabulary.word;

        positionElement.textContent = `${currentIndex + 1} / ${vocabularies.length}`;

        audioElement.src = vocabulary.audioUrl || "";

        selectedMeaning = null;
        selectedListeningWord = null;

        meaningResult.textContent = "";
        listeningResult.textContent = "";


        renderMeaningAnswers();

        renderListeningAnswers();

        updateNavigation();
    }

    function renderMeaningAnswers() {

        const current = currentVocabulary();
        const wrongAnswers = vocabularies.filter(x => x.vocabularyId !== current.vocabularyId).map(x => x.meaning);
        const options = shuffle([current.meaning, ...shuffle(wrongAnswers).slice(0, 3)]);

        meaningGrid.innerHTML = "";


        options.forEach((meaning, index) => {
            const button = document.createElement("button");
            button.type = "button";
            button.dataset.value = meaning;
            button.textContent = `${String.fromCharCode(65 + index)}. ${meaning}`;
            button.addEventListener("click", () => {
                meaningGrid
                    .querySelectorAll("button")
                    .forEach(x =>
                        x.classList.remove("selected"));


                button.classList.add("selected");

                selectedMeaning = meaning;
            });


            meaningGrid.appendChild(button);
        });
    }

    function checkMeaning() {

        if (!selectedMeaning) {

            meaningResult.textContent =
                "Vui lòng chọn đáp án.";

            return;
        }

        const current =
            currentVocabulary();


        meaningGrid
            .querySelectorAll("button")
            .forEach(button => {

                button.classList.remove(
                    "selected-correct",
                    "selected-wrong"
                );

                if (
                    button.dataset.value === current.meaning
                ) {
                    button.classList.add(
                        "selected-correct"
                    );
                }

                else if (
                    button.dataset.value === selectedMeaning
                ) {
                    button.classList.add(
                        "selected-wrong"
                    );
                }
            });


        if (selectedMeaning === current.meaning) {

            meaningResult.textContent =
                "Chính xác.";

        } else {

            meaningResult.textContent =
                `Đáp án đúng: ${current.meaning}`;
        }
    }

    function renderListeningAnswers() {

        const current =
            currentVocabulary();

        const wrongWords =
            vocabularies
                .filter(x =>
                    x.vocabularyId !== current.vocabularyId)
                .map(x => x.word);


        const options =
            shuffle([
                current.word,
                ...shuffle(wrongWords).slice(0, 3)
            ]);


        listeningGrid.innerHTML = "";


        options.forEach((word, index) => {

            const button =
                document.createElement("button");

            button.type = "button";

            button.dataset.value = word;

            button.textContent =
                `${String.fromCharCode(65 + index)}. ${word}`;


            button.addEventListener("click", () => {

                listeningGrid
                    .querySelectorAll("button")
                    .forEach(x =>
                        x.classList.remove("selected"));


                button.classList.add("selected");

                selectedListeningWord = word;
            });


            listeningGrid.appendChild(button);
        });
    }

    function checkListening() {

        if (!selectedListeningWord) {

            listeningResult.textContent =
                "Vui lòng chọn đáp án.";

            return;
        }


        const current =
            currentVocabulary();


        listeningGrid
            .querySelectorAll("button")
            .forEach(button => {

                button.classList.remove(
                    "selected-correct",
                    "selected-wrong"
                );


                if (
                    button.dataset.value === current.word
                ) {
                    button.classList.add(
                        "selected-correct"
                    );
                }

                else if (
                    button.dataset.value === selectedListeningWord
                ) {
                    button.classList.add(
                        "selected-wrong"
                    );
                }

            });


        if (
            selectedListeningWord === current.word
        ) {

            listeningResult.textContent =
                "Chính xác.";

        } else {

            listeningResult.textContent =
                `Đáp án đúng: ${current.word}`;
        }
    }

    function playAudio() {

        const current =
            currentVocabulary();

        if (!current.audioUrl) {
            return;
        }

        audioElement.currentTime = 0;

        audioElement.play();
    }


    function nextVocabulary() {

        if (
            currentIndex <
            vocabularies.length - 1
        ) {

            currentIndex++;

            renderVocabulary();
        }
    }


    function previousVocabulary() {

        if (currentIndex > 0) {

            currentIndex--;

            renderVocabulary();
        }
    }


    function updateNavigation() {

        document
            .getElementById("previous-vocabulary")
            .disabled =
            currentIndex === 0;


        document
            .getElementById("next-vocabulary")
            .disabled =
            currentIndex === vocabularies.length - 1;
    }

    document
        .getElementById("play-word-audio")
        .addEventListener(
            "click",
            playAudio
        );


    document
        .getElementById("play-example-audio")
        .addEventListener(
            "click",
            playAudio
        );


    document
        .getElementById("play-listening-audio")
        .addEventListener(
            "click",
            playAudio
        );


    document
        .getElementById("sound-button")
        .addEventListener(
            "click",
            playAudio
        );


    document
        .getElementById("listen-again")
        .addEventListener(
            "click",
            playAudio
        );


    document
        .getElementById("check-meaning-answer")
        .addEventListener(
            "click",
            checkMeaning
        );


    document
        .getElementById("check-listening-answer")
        .addEventListener(
            "click",
            checkListening
        );


    document
        .getElementById("previous-vocabulary")
        .addEventListener(
            "click",
            previousVocabulary
        );


    document
        .getElementById("next-vocabulary")
        .addEventListener(
            "click",
            nextVocabulary
        );


    renderVocabulary();
});