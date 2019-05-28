# run: jupyter nbconvert --to python train_rotation_order.ipynb --template=python_script.tpl
{% extends 'python.tpl'%}
{% block in_prompt %}{% endblock in_prompt %}
{% block input %}{{cell.source}}
{% endblock input %}