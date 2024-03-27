import sys
import io
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity

# -*- coding: utf-8 -*-
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')


def secimlik():
    secim = (input())
    data = pd.read_csv('C:\\Users\\AhmetCan\\source\\repos\\File_Upload\\File_Upload\\wwwroot\\img\\' + secim,
                       encoding='ISO-8859-9')
    return data, secim

def cossim():
    data, secim = secimlik()

    def create_combined_features(row, selected_features):
        return ' '.join([str(row[feature]) for feature in selected_features])

    selected_features = input().split(',')

    # Kullanıcının seçtiği özelliklere göre 'combined_features' sütununu oluştur
    data['combined_features'] = data.apply(lambda row: create_combined_features(row, selected_features), axis=1)

    # Birleştirilmiş özellikleri vektörleştir
    vectorizer = TfidfVectorizer()
    tfidf_matrix = vectorizer.fit_transform(data['combined_features'])

    # Kosinüs benzerliği hesapla
    cosine_sim = cosine_similarity(tfidf_matrix, tfidf_matrix)

    def get_data_recommendations(title, top_n=10, ):
        p_name = input()
        p_type = input()
        if p_type == "evet":
            index = data[data[p_name] == int(title)].index[0]
        else:
            index = data[data[p_name] == title].index[0]
        similarity_scores = list(enumerate(cosine_sim[index]))
        similarity_scores = sorted(similarity_scores, key=lambda x: x[1], reverse=True)
        top_recommendations = similarity_scores[1:top_n + 1]
        data1 = p_name
        # data2 = data[0]
        # data3 = data[1]

        recommended = [data[data1][top_data[0]] for top_data
                       in top_recommendations]
        # data[data2][top_data[0]], data[data3][top_data[0]]]
        return recommended, [score[1] for score in top_recommendations]
    # Kullanıcıdan film başlığı girdisi al input("Aradığınızı Girin: ")
    title = input()

    # Kullanıcının girdisine göre film önerilerini al
    recommendations, similarity_scores = get_data_recommendations(title, top_n=10)

    return recommendations


cossim_ornek = cossim()


for sonuclar in cossim_ornek:
  print(str(sonuclar))