import './AutocompleteDropdown.scss';

interface Props {
  items: string[];
  onSelect: (name: string) => void;
}

export function AutocompleteDropdown({ items, onSelect }: Props) {
  return (
    <ul className="autocomplete-dropdown">
      {items.map(item => (
        <li
          key={item}
          className="autocomplete-dropdown__item"
          onMouseDown={() => onSelect(item)}
        >
          {item}
        </li>
      ))}
    </ul>
  );
}
