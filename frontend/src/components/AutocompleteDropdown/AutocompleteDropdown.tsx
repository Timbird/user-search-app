import './AutocompleteDropdown.scss';

interface Props {
  items: string[];
  query: string;
  onSelect: (name: string) => void;
}

function highlightMatch(text: string, query: string) {
  if (!query) return text;
  const idx = text.toLowerCase().indexOf(query.toLowerCase());
  if (idx === -1) return text;
  return (
    <>
      {text.slice(0, idx)}
      <mark className="autocomplete-dropdown__highlight">
        {text.slice(idx, idx + query.length)}
      </mark>
      {text.slice(idx + query.length)}
    </>
  );
}

export function AutocompleteDropdown({ items, query, onSelect }: Props) {
  return (
    <ul className="autocomplete-dropdown">
      {items.map(item => (
        <li
          key={item}
          className="autocomplete-dropdown__item"
          onMouseDown={() => onSelect(item)}
        >
          {highlightMatch(item, query)}
        </li>
      ))}
    </ul>
  );
}
